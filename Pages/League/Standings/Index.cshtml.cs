using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.League.Standings;

public class IndexModel : PageModel
{
    private readonly ILeagueManagerWorkspaceService _workspace;

    public IndexModel(ILeagueManagerWorkspaceService workspace) => _workspace = workspace;

    public LeagueWorkspace Workspace { get; private set; } = default!;

    public async Task OnGetAsync(CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
    }
}
