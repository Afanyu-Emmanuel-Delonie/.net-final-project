using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.RefereeManager;

public class AvailabilityModel : PageModel
{
    private readonly IRefereeManagerWorkspaceService _workspace;

    public AvailabilityModel(IRefereeManagerWorkspaceService workspace) => _workspace = workspace;

    public RefereeManagerWorkspace Workspace { get; private set; } = default!;

    public async Task OnGetAsync(CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
    }

    public static string CellClass(string cell) => cell switch
    {
        "available" => "avail-cell--available",
        "assigned" => "avail-cell--assigned",
        _ => "avail-cell--unavailable",
    };

    public static string CellLabel(string cell) => cell switch
    {
        "available" => "Available",
        "assigned" => "Assigned",
        _ => "Unavailable",
    };
}
