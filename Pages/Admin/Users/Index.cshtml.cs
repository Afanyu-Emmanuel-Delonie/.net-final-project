using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Admin.Users;

public class IndexModel : PageModel
{
    private readonly IAdminWorkspaceService _workspace;

    public IndexModel(IAdminWorkspaceService workspace) => _workspace = workspace;

    public AdminWorkspace Workspace { get; private set; } = default!;

    public static readonly string[] Roles = { "Federation admin", "League manager", "Club manager", "Referee" };
    public static readonly string[] Clubs = { "APR FC", "Rayon Sports", "Police FC", "Kiyovu SC", "Mukura VS", "Marines FC" };

    public async Task OnGetAsync(CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
    }
}
