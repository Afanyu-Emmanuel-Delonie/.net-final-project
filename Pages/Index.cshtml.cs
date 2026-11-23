using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages;

public class IndexModel : PageModel
{
    private readonly ILeagueSnapshotService _league;

    public IndexModel(ILeagueSnapshotService league) => _league = league;

    public LeagueSnapshot Snapshot { get; private set; } = default!;

    public async Task OnGetAsync(CancellationToken ct)
    {
        Snapshot = await _league.GetCurrentAsync(ct);
    }

    public static string Outcome(ResultItem r) =>
        r.HomeGoals > r.AwayGoals ? $"{r.Home.Name} won" :
        r.HomeGoals < r.AwayGoals ? $"{r.Away.Name} won" : "Draw";
}