using FERW.Models;
using FERW.Services.Players;
using DemoRegistration = FERW.Services.PlayerRegistration;
using DemoWindow = FERW.Services.RegistrationWindow;

namespace FERW.Services;

// Admin workspace with player registrations and registration windows read from the database.
// Transfers, licensing, rules and users still come from the demo data until those modules are built.
public class DbAdminWorkspaceService : IAdminWorkspaceService
{
    private readonly DemoAdminWorkspaceService _demo;
    private readonly IRegistrationReviewService _reviews;
    private readonly IRegistrationWindowService _windows;
    private readonly TimeProvider _clock;
    private readonly LeagueTime _leagueTime;

    public DbAdminWorkspaceService(DemoAdminWorkspaceService demo, IRegistrationReviewService reviews,
        IRegistrationWindowService windows, TimeProvider clock, LeagueTime leagueTime)
    {
        _demo = demo;
        _reviews = reviews;
        _windows = windows;
        _clock = clock;
        _leagueTime = leagueTime;
    }

    public async Task<AdminWorkspace> GetWorkspaceAsync(CancellationToken ct = default)
    {
        var demo = await _demo.GetWorkspaceAsync(ct);
        var rows = await _reviews.GetRegistrationsAsync(ct);
        var windows = await _windows.GetWindowsAsync(ct);
        var now = _clock.GetUtcNow().UtcDateTime;

        int WaitDays(DateTime submittedUtc) => Math.Max(0, (int)(now - submittedUtc).TotalDays);

        var registrations = rows.Select(r => new DemoRegistration(
                r.Id.ToString(), r.PlayerName, r.ClubName, r.Category.Label(),
                _leagueTime.ToLeagueTime(r.SubmittedAt).ToString("d MMM yyyy"),
                $"{WaitDays(r.SubmittedAt)} days", r.Status.ToString(),
                r.DateOfBirth.ToString("d MMM yyyy"), r.Position.ToString(), r.IsForeign,
                r.HasContract, r.HasMedical, r.HasInsurance, r.FeeDue ? r.FeePaid : null))
            .ToList();

        var windowRows = windows.Select(w => new DemoWindow(
                w.Id.ToString(), w.Category.Label(), w.StartDate.ToString("d MMM yyyy"), w.EndDate.ToString("d MMM yyyy"),
                w.IsOpen ? "Open" : w.IsUpcoming ? "Upcoming" : "Closed"))
            .ToList();

        var pending = rows.Where(r => r.Status == RegistrationStatus.Pending).ToList();
        var summary = demo.Summary with
        {
            PendingRegistrations = pending.Count,
            OldestRegistrationWait = pending.Count == 0 ? 0 : pending.Max(r => WaitDays(r.SubmittedAt)),
        };

        return demo with { Summary = summary, Registrations = registrations, Windows = windowRows };
    }
}
