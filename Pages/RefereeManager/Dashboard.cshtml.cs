using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.RefereeManager;

public class DashboardModel : PageModel
{
    private readonly IRefereeManagerWorkspaceService _workspace;

    public DashboardModel(IRefereeManagerWorkspaceService workspace) => _workspace = workspace;

    public RefereeManagerWorkspace Workspace { get; private set; } = default!;

    public async Task OnGetAsync(CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
    }
}
