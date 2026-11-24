using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.RefereeManager.Roster;

public class IndexModel : PageModel
{
    private readonly IRefereeManagerWorkspaceService _workspace;

    public IndexModel(IRefereeManagerWorkspaceService workspace) => _workspace = workspace;

    public RefereeManagerWorkspace Workspace { get; private set; } = default!;
    public string Filter { get; private set; } = "all";
    public IReadOnlyList<RefereeRecord> FilteredReferees { get; private set; } = Array.Empty<RefereeRecord>();

    public async Task OnGetAsync(string? filter, CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
        Filter = filter switch
        {
            "active" or "unavailable" or "fifa" or "applications" => filter,
            _ => "all",
        };

        FilteredReferees = Filter switch
        {
            "active" => Workspace.Referees.Where(r => r.Status == "Active").ToList(),
            "unavailable" => Workspace.Referees.Where(r => r.Status == "Unavailable").ToList(),
            "fifa" => Workspace.Referees.Where(r => r.Certification == "FIFA-listed").ToList(),
            "applications" => Workspace.Referees.Where(r => r.Status == "Pending").ToList(),
            _ => Workspace.Referees,
        };
    }

    public static string Initials(string name) =>
        string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(p => p[0])).ToUpperInvariant() is { Length: > 2 } s
            ? s[..2]
            : string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(p => p[0])).ToUpperInvariant();

    public static string StatusClass(string status) => status switch
    {
        "Active" => "staff-status--approved",
        "Pending" => "staff-status--pending",
        _ => "staff-status--warn",
    };

    public static string CertClass(string cert) => cert switch
    {
        "FIFA-listed" => "cert-badge--fifa",
        "National" => "cert-badge--national",
        _ => "cert-badge--regional",
    };
}
