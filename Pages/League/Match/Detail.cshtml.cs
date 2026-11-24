using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.League.Match;

public class DetailModel : PageModel
{
    private readonly ILeagueManagerWorkspaceService _workspace;

    public DetailModel(ILeagueManagerWorkspaceService workspace) => _workspace = workspace;

    public LeagueWorkspace Workspace { get; private set; } = default!;
    public MatchRecord? Match { get; private set; }

    public async Task OnGetAsync(string? id, CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
        Match = (id is null ? null : Workspace.Matches.FirstOrDefault(m => m.Id == id)) ?? Workspace.Matches.FirstOrDefault();
    }
}
