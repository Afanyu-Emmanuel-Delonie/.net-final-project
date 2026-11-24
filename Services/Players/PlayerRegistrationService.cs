// Block-scoped namespace so FERW.Models types win over the same-named demo records in FERW.Services.
namespace FERW.Services.Players
{
using FERW.Data;
using FERW.Models;
using Microsoft.EntityFrameworkCore;

public record RegistrationRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Nationality,
    string NationalIdOrPassport,
    string? Phone,
    Position Position,
    int? PreferredShirtNumber,
    RegistrationType RegistrationType,
    int RegistrationWindowId,
    DateOnly ContractStartDate,
    DateOnly ContractEndDate);

public record RegistrationFiles(IFormFile? Contract, IFormFile? MedicalReport, IFormFile? Insurance, IFormFile? Photo);

// Field names match the request/file property names so the page can map errors onto its inputs.
public record SubmitResult(int? RegistrationId, IReadOnlyList<(string Field, string Message)> Errors)
{
    public bool Succeeded => RegistrationId is not null;
}

public record OpenWindow(int Id, PlayerCategory Category, DateOnly EndDate);

public record ClubSubmission(
    int Id, string PlayerName, PlayerCategory Category, DateTime SubmittedAt,
    RegistrationStatus Status, bool IsForeign, int? ForeignFeeRwf, string? Remarks);

public interface IPlayerRegistrationService
{
    Task<IReadOnlyList<OpenWindow>> GetOpenWindowsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ClubSubmission>> GetClubSubmissionsAsync(int clubId, int take = 10, CancellationToken ct = default);
    Task<SubmitResult> SubmitAsync(int clubId, string submittedBy, RegistrationRequest request, RegistrationFiles files, CancellationToken ct = default);
}

public class PlayerRegistrationService : IPlayerRegistrationService
{
    public const long MaxDocumentBytes = 5 * 1024 * 1024;
    public const long MaxPhotoBytes = 2 * 1024 * 1024;
    public const int MinimumAge = 15;

    private static readonly string[] DocumentExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
    private static readonly string[] PhotoExtensions = { ".jpg", ".jpeg", ".png" };

    private readonly AppDbContext _db;
    private readonly IFileStorage _storage;
    private readonly TimeProvider _clock;

    public PlayerRegistrationService(AppDbContext db, IFileStorage storage, TimeProvider clock)
    {
        _db = db;
        _storage = storage;
        _clock = clock;
    }

    private DateOnly Today => DateOnly.FromDateTime(_clock.GetLocalNow().DateTime);

    public async Task<IReadOnlyList<OpenWindow>> GetOpenWindowsAsync(CancellationToken ct = default)
    {
        var today = Today;
        return await _db.RegistrationWindows
            .Where(w => w.StartDate <= today && w.EndDate >= today)
            .OrderBy(w => w.Category)
            .Select(w => new OpenWindow(w.Id, w.Category, w.EndDate))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ClubSubmission>> GetClubSubmissionsAsync(int clubId, int take = 10, CancellationToken ct = default)
    {
        return await _db.PlayerRegistrations
            .Where(r => r.ClubId == clubId)
            .OrderByDescending(r => r.SubmittedAt)
            .Take(take)
            .Select(r => new ClubSubmission(
                r.Id, r.Player.FirstName + " " + r.Player.LastName, r.RegistrationWindow.Category,
                r.SubmittedAt, r.Status, r.Player.IsForeign, r.ForeignFeeRwf, r.Remarks))
            .ToListAsync(ct);
    }

    public async Task<SubmitResult> SubmitAsync(int clubId, string submittedBy, RegistrationRequest req, RegistrationFiles files, CancellationToken ct = default)
    {
        var errors = new List<(string, string)>();
        var today = Today;

        // FR2.2: only while the chosen window is open (checked by date on the server, never trusted from the form).
        var window = await _db.RegistrationWindows.FindAsync(new object[] { req.RegistrationWindowId }, ct);
        if (window is null)
            errors.Add((nameof(req.RegistrationWindowId), "Choose a registration window."));
        else if (!window.IsOpenOn(today))
            errors.Add((nameof(req.RegistrationWindowId), $"The {window.Category.Label()} window is closed. Registrations are only accepted while a window is open."));

        // TODO: once the club module adds the Club table, also reject clubs that are inactive or unlicensed.

        // Player details.
        if (req.DateOfBirth >= today)
            errors.Add((nameof(req.DateOfBirth), "Date of birth must be in the past."));
        else if (req.DateOfBirth > today.AddYears(-MinimumAge))
            errors.Add((nameof(req.DateOfBirth), $"Players must be at least {MinimumAge} years old."));

        if (!Nationalities.All.Contains(req.Nationality))
            errors.Add((nameof(req.Nationality), "Choose a nationality from the list."));

        if (req.ContractEndDate <= req.ContractStartDate)
            errors.Add((nameof(req.ContractEndDate), "Contract end date must be after the start date."));

        // FR2.3: all three documents are required.
        ValidateFile(files.Contract, nameof(files.Contract), DocumentType.Contract.Label(), DocumentExtensions, MaxDocumentBytes, required: true, errors);
        ValidateFile(files.MedicalReport, nameof(files.MedicalReport), DocumentType.MedicalReport.Label(), DocumentExtensions, MaxDocumentBytes, required: true, errors);
        ValidateFile(files.Insurance, nameof(files.Insurance), DocumentType.Insurance.Label(), DocumentExtensions, MaxDocumentBytes, required: true, errors);
        ValidateFile(files.Photo, nameof(files.Photo), "Photo", PhotoExtensions, MaxPhotoBytes, required: false, errors);

        // A player can only have one live registration. Rejected players may be resubmitted.
        var idNumber = req.NationalIdOrPassport.Trim().ToUpperInvariant();
        var player = await _db.Players
            .Include(p => p.Registrations)
            .FirstOrDefaultAsync(p => p.NationalIdOrPassport == idNumber, ct);
        if (player is not null && player.Registrations.Any(r => r.Status != RegistrationStatus.Rejected))
            errors.Add((nameof(req.NationalIdOrPassport), "This player already has a pending or approved registration. Use a transfer to move a registered player."));

        if (errors.Count > 0)
            return new SubmitResult(null, errors);

        // FR2.4: foreign status from nationality; fee comes from the league rules table.
        var isForeign = Nationalities.IsForeign(req.Nationality);
        int? fee = null;
        if (isForeign)
        {
            fee = await _db.LeagueRules
                .Where(r => r.Key == LeagueRule.ForeignPlayerFee)
                .Select(r => (int?)r.Value)
                .FirstOrDefaultAsync(ct) ?? 0;
        }

        var saved = new List<string>();
        var folder = $"registrations/{Guid.NewGuid():N}";
        try
        {
            player ??= new Player { NationalIdOrPassport = idNumber };
            player.FirstName = req.FirstName.Trim();
            player.LastName = req.LastName.Trim();
            player.DateOfBirth = req.DateOfBirth;
            player.Nationality = req.Nationality;
            player.Phone = string.IsNullOrWhiteSpace(req.Phone) ? null : req.Phone.Trim();
            player.Position = req.Position;
            player.IsForeign = isForeign;
            if (player.Id == 0)
                _db.Players.Add(player);

            if (files.Photo is { Length: > 0 })
            {
                player.PhotoPath = await _storage.SaveAsync(files.Photo, folder, ct);
                saved.Add(player.PhotoPath);
            }

            var now = _clock.GetUtcNow().UtcDateTime;
            var registration = new PlayerRegistration
            {
                Player = player,
                ClubId = clubId,
                RegistrationWindowId = window!.Id,
                RegistrationType = req.RegistrationType,
                PreferredShirtNumber = req.PreferredShirtNumber,
                ContractStartDate = req.ContractStartDate,
                ContractEndDate = req.ContractEndDate,
                ForeignFeeRwf = fee,
                Status = RegistrationStatus.Pending,
                SubmittedAt = now,
                SubmittedBy = submittedBy,
            };

            foreach (var (type, file) in new[]
            {
                (DocumentType.Contract, files.Contract!),
                (DocumentType.MedicalReport, files.MedicalReport!),
                (DocumentType.Insurance, files.Insurance!),
            })
            {
                var path = await _storage.SaveAsync(file, folder, ct);
                saved.Add(path);
                registration.Documents.Add(new PlayerDocument
                {
                    Type = type,
                    OriginalFileName = Path.GetFileName(file.FileName),
                    StoredPath = path,
                    ContentType = file.ContentType,
                    SizeBytes = file.Length,
                    UploadedAt = now,
                });
            }

            _db.PlayerRegistrations.Add(registration);

            // A single SaveChanges runs in one transaction: player, registration and documents are saved together or not at all.
            await _db.SaveChangesAsync(ct);
            return new SubmitResult(registration.Id, Array.Empty<(string, string)>());
        }
        catch
        {
            foreach (var path in saved)
                _storage.Delete(path);
            throw;
        }
    }

    private static void ValidateFile(IFormFile? file, string field, string label, string[] allowedExtensions,
        long maxBytes, bool required, List<(string, string)> errors)
    {
        if (file is null || file.Length == 0)
        {
            if (required)
                errors.Add((field, $"{label} is required."));
            return;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext) || !HasExpectedSignature(file, ext))
        {
            var names = allowedExtensions.Where(e => e != ".jpeg").Select(e => e.TrimStart('.').ToUpperInvariant()).ToList();
            var allowed = names.Count == 1 ? names[0] : $"{string.Join(", ", names.SkipLast(1))} or {names[^1]}";
            errors.Add((field, $"{label} must be a {allowed} file."));
            return;
        }

        if (file.Length > maxBytes)
            errors.Add((field, $"{label} must be {maxBytes / (1024 * 1024)} MB or smaller."));
    }

    // Checks the file's first bytes so a renamed executable can't pass as a PDF or image.
    private static bool HasExpectedSignature(IFormFile file, string ext)
    {
        Span<byte> head = stackalloc byte[4];
        using var stream = file.OpenReadStream();
        if (stream.Read(head) < 4)
            return false;

        return ext switch
        {
            ".pdf" => head.SequenceEqual("%PDF"u8),
            ".png" => head.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47 }),
            ".jpg" or ".jpeg" => head[0] == 0xFF && head[1] == 0xD8 && head[2] == 0xFF,
            _ => false
        };
    }
}
}
