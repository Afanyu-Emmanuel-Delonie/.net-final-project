namespace FERW.Services;

public record LeagueSnapshot(
    string SeasonName,
    int MatchdayPlayed,
    bool IsDemoData,
    FixtureItem? NextMatch,
    IReadOnlyList<ResultItem> LatestResults,
    IReadOnlyList<StandingRow> Standings,
    IReadOnlyList<FixtureItem> UpcomingFixtures);

public record ClubRef(string Name, string ShortCode);

public record ResultItem(ClubRef Home, ClubRef Away, int HomeGoals, int AwayGoals, DateTime PlayedOn, string Stadium);

public record FixtureItem(ClubRef Home, ClubRef Away, DateTime KickOff, string Stadium);

public record StandingRow(int Position, ClubRef Club, int Played, int Won, int Drawn, int Lost, int GoalsFor, int GoalsAgainst)
{
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points => Won * 3 + Drawn;
}

public interface ILeagueSnapshotService
{
    Task<LeagueSnapshot> GetCurrentAsync(CancellationToken ct = default);
}