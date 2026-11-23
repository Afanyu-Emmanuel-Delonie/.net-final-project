using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Admin;

public class DashboardModel : PageModel
{
    private readonly IAdminWorkspaceService _workspace;

    public DashboardModel(IAdminWorkspaceService workspace) => _workspace = workspace;

    public AdminWorkspace Workspace { get; private set; } = default!;

    public async Task OnGetAsync(CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
    }
}
