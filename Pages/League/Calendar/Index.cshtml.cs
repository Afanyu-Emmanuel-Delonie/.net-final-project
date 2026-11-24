using System.Text.Json;
using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.League.Calendar;

public class IndexModel : PageModel
{
    private readonly ILeagueManagerWorkspaceService _workspace;

    public IndexModel(ILeagueManagerWorkspaceService workspace) => _workspace = workspace;

    public static readonly string[] Stadiums = { "Kigali Pelé Stadium", "Huye Stadium", "Umuganda Stadium" };

    public LeagueWorkspace Workspace { get; private set; } = default!;
    public string Round { get; private set; } = "Round 1";
    public string View { get; private set; } = "matchday";
    public string? SelectedClub { get; private set; }
    public IReadOnlyList<MatchRecord> RoundMatches { get; private set; } = Array.Empty<MatchRecord>();
    public IReadOnlyList<MatchRecord> ClubMatches { get; private set; } = Array.Empty<MatchRecord>();
    public string MatchesJson { get; private set; } = "[]";

    public async Task OnGetAsync(string? round, string? view, string? club, CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
        Round = round == "2" ? "Round 2" : "Round 1";
        View = view == "club" ? "club" : "matchday";
        RoundMatches = Workspace.Matches.Where(m => m.Round == Round).ToList();

        SelectedClub = club ?? Workspace.SeasonClubs.Select(c => c.Code).FirstOrDefault();
        ClubMatches = RoundMatches.Where(m => m.HomeCode == SelectedClub || m.AwayCode == SelectedClub).ToList();

        MatchesJson = JsonSerializer.Serialize(Workspace.Matches
            .Where(m => m.Date is not null)
            .Select(m => new
            {
                m.Id,
                home = m.HomeCode,
                away = m.AwayCode,
                m.Stadium,
                date = m.Date!.Value.ToString("yyyy-MM-dd"),
                kickoff = m.Kickoff!.Value.ToString("HH:mm"),
            }));
    }
}
