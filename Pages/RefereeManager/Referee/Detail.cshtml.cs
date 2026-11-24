using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.RefereeManager.Referee;

public class DetailModel : PageModel
{
    private readonly IRefereeManagerWorkspaceService _workspace;

    public DetailModel(IRefereeManagerWorkspaceService workspace) => _workspace = workspace;

    public RefereeRecord? Referee { get; private set; }
    public string UnassignedMatchLabel { get; private set; } = "";

    public async Task OnGetAsync(string? id, CancellationToken ct)
    {
        var workspace = await _workspace.GetWorkspaceAsync(ct);
        UnassignedMatchLabel = workspace.UnassignedMatchLabel;
        Referee = (id is null ? null : workspace.Referees.FirstOrDefault(r => r.Id == id))
            ?? workspace.Referees.FirstOrDefault();
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
