// Block-scoped namespace so FERW.Models types win over the same-named demo records in FERW.Services.
namespace FERW.Services.Players
{
using FERW.Data;
using FERW.Models;
using Microsoft.EntityFrameworkCore;

public record ReviewResult(bool Succeeded, string Message)
{
    public static ReviewResult Ok(string message) => new(true, message);
    public static ReviewResult Fail(string message) => new(false, message);
}

public record AdminRegistrationRow(
    int Id, string PlayerName, string ClubName, PlayerCategory Category, DateTime SubmittedAt,
    RegistrationStatus Status, DateOnly DateOfBirth, Position Position, bool IsForeign,
    bool HasContract, bool HasMedical, bool HasInsurance, bool FeeDue, bool FeePaid, string? Remarks);

// FERWAFA's side of player registration: the review queue and the approve / reject decisions.
public interface IRegistrationReviewService
{
    Task<IReadOnlyList<AdminRegistrationRow>> GetRegistrationsAsync(CancellationToken ct = default);
    Task<ReviewResult> ApproveAsync(int registrationId, string reviewer, CancellationToken ct = default);
    Task<ReviewResult> RejectAsync(int registrationId, string reviewer, string? reason, CancellationToken ct = default);
    Task<ReviewResult> MarkFeePaidAsync(int registrationId, string reviewer, CancellationToken ct = default);
}

public class RegistrationReviewService : IRegistrationReviewService
{
    public const int MaxReasonLength = 500;

    private readonly AppDbContext _db;
    private readonly TimeProvider _clock;

    public RegistrationReviewService(AppDbContext db, TimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<IReadOnlyList<AdminRegistrationRow>> GetRegistrationsAsync(CancellationToken ct = default)
    {
        return await _db.PlayerRegistrations
            .OrderBy(r => r.Status == RegistrationStatus.Pending ? 0 : 1)
            .ThenBy(r => r.SubmittedAt)
            .Select(r => new AdminRegistrationRow(
                r.Id, r.Details.FirstName + " " + r.Details.LastName, r.ClubName, r.RegistrationWindow.Category,
                r.SubmittedAt, r.Status, r.Details.DateOfBirth, r.Details.Position, r.Details.IsForeign,
                r.Documents.Any(d => d.Type == DocumentType.Contract),
                r.Documents.Any(d => d.Type == DocumentType.MedicalReport),
                r.Documents.Any(d => d.Type == DocumentType.Insurance),
                r.ForeignFeeRwf != null, r.FeePaidAt != null, r.Remarks))
            .ToListAsync(ct);
    }

    public async Task<ReviewResult> ApproveAsync(int registrationId, string reviewer, CancellationToken ct = default)
    {
        var registration = await _db.PlayerRegistrations
            .Include(r => r.Player)
            .Include(r => r.Documents)
            .FirstOrDefaultAsync(r => r.Id == registrationId, ct);
        if (registration is null)
            return ReviewResult.Fail("That registration no longer exists.");
        if (registration.Status != RegistrationStatus.Pending)
            return ReviewResult.Fail("This registration has already been reviewed.");

        var missing = Enum.GetValues<DocumentType>().Where(t => registration.Documents.All(d => d.Type != t)).ToList();
        if (missing.Count > 0)
            return ReviewResult.Fail($"Missing documents: {string.Join(", ", missing.Select(t => t.Label()))}.");
        if (registration.ForeignFeeRwf is not null && registration.FeePaidAt is null)
            return ReviewResult.Fail("The foreign-player fee has not been received yet.");

        var now = _clock.GetUtcNow().UtcDateTime;
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // The status change only applies while the row is still pending, so two reviewers can't both decide it.
        var changed = await _db.PlayerRegistrations
            .Where(r => r.Id == registrationId && r.Status == RegistrationStatus.Pending)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, RegistrationStatus.Approved)
                .SetProperty(r => r.ReviewedAt, now)
                .SetProperty(r => r.ReviewedBy, reviewer), ct);
        if (changed == 0)
            return ReviewResult.Fail("This registration has already been reviewed.");

        var player = registration.Player;
        player.ClubId = registration.ClubId;
        player.LicenseNumber ??= $"FRW-{now.Year}-{player.Id:D5}";
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return ReviewResult.Ok($"{registration.Details.FullName} is approved with license {player.LicenseNumber}.");
    }

    public async Task<ReviewResult> RejectAsync(int registrationId, string reviewer, string? reason, CancellationToken ct = default)
    {
        reason = reason?.Trim();
        if (string.IsNullOrEmpty(reason))
            return ReviewResult.Fail("Give a reason so the club knows what to fix.");
        if (reason.Length > MaxReasonLength)
            return ReviewResult.Fail($"Keep the reason to {MaxReasonLength} characters or fewer.");

        var now = _clock.GetUtcNow().UtcDateTime;
        var changed = await _db.PlayerRegistrations
            .Where(r => r.Id == registrationId && r.Status == RegistrationStatus.Pending)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, RegistrationStatus.Rejected)
                .SetProperty(r => r.Remarks, reason)
                .SetProperty(r => r.ReviewedAt, now)
                .SetProperty(r => r.ReviewedBy, reviewer), ct);

        return changed == 0
            ? ReviewResult.Fail("This registration has already been reviewed.")
            : ReviewResult.Ok("Registration rejected. The club can see your reason and resubmit.");
    }

    public async Task<ReviewResult> MarkFeePaidAsync(int registrationId, string reviewer, CancellationToken ct = default)
    {
        var now = _clock.GetUtcNow().UtcDateTime;
        var changed = await _db.PlayerRegistrations
            .Where(r => r.Id == registrationId && r.Status == RegistrationStatus.Pending
                        && r.ForeignFeeRwf != null && r.FeePaidAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.FeePaidAt, now), ct);

        return changed == 0
            ? ReviewResult.Fail("No unpaid foreign-player fee is due on this registration.")
            : ReviewResult.Ok("Foreign-player fee recorded as received.");
    }
}

public record WindowRow(int Id, PlayerCategory Category, DateOnly StartDate, DateOnly EndDate, bool IsOpen, bool IsUpcoming);

// Registration windows are managed by FERWAFA in the app, so new seasons don't need a code change.
public interface IRegistrationWindowService
{
    Task<IReadOnlyList<WindowRow>> GetWindowsAsync(CancellationToken ct = default);
    Task<ReviewResult> SaveAsync(int? id, PlayerCategory category, DateOnly start, DateOnly end, CancellationToken ct = default);
}

public class RegistrationWindowService : IRegistrationWindowService
{
    private readonly AppDbContext _db;
    private readonly TimeProvider _clock;
    private readonly LeagueTime _leagueTime;

    public RegistrationWindowService(AppDbContext db, TimeProvider clock, LeagueTime leagueTime)
    {
        _db = db;
        _clock = clock;
        _leagueTime = leagueTime;
    }

    public async Task<IReadOnlyList<WindowRow>> GetWindowsAsync(CancellationToken ct = default)
    {
        var today = _leagueTime.Today(_clock);
        var windows = await _db.RegistrationWindows
            .OrderByDescending(w => w.EndDate)
            .ThenBy(w => w.Category)
            .ToListAsync(ct);
        return windows
            .Select(w => new WindowRow(w.Id, w.Category, w.StartDate, w.EndDate, w.IsOpenOn(today), w.StartDate > today))
            .ToList();
    }

    public async Task<ReviewResult> SaveAsync(int? id, PlayerCategory category, DateOnly start, DateOnly end, CancellationToken ct = default)
    {
        if (!Enum.IsDefined(category))
            return ReviewResult.Fail("Choose a category from the list.");
        if (end < start)
            return ReviewResult.Fail("The end date must be on or after the start date.");

        var overlaps = await _db.RegistrationWindows.AnyAsync(w =>
            w.Id != id && w.Category == category && w.StartDate <= end && w.EndDate >= start, ct);
        if (overlaps)
            return ReviewResult.Fail($"These dates overlap another {category.Label()} window.");

        RegistrationWindow window;
        if (id is int existingId)
        {
            var found = await _db.RegistrationWindows.FindAsync(new object[] { existingId }, ct);
            if (found is null)
                return ReviewResult.Fail("That window no longer exists.");
            window = found;
            if (window.Category != category
                && await _db.PlayerRegistrations.AnyAsync(r => r.RegistrationWindowId == existingId, ct))
                return ReviewResult.Fail("Registrations were submitted in this window, so its category can't change. Create a new window instead.");
        }
        else
        {
            window = new RegistrationWindow();
            _db.RegistrationWindows.Add(window);
        }

        window.Category = category;
        window.StartDate = start;
        window.EndDate = end;
        await _db.SaveChangesAsync(ct);
        return ReviewResult.Ok($"{category.Label()} window saved: {start:d MMM yyyy} to {end:d MMM yyyy}.");
    }
}
}
