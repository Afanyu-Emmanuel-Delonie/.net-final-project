namespace FERW.Services;

public record MatchRecord(
    string Id, string Round, int Matchday, string HomeCode, string AwayCode, string Stadium,
    DateOnly? Date, TimeOnly? Kickoff, string DisplayTime, string Status, string? Score,
    string? Referee, bool SquadHome, bool SquadAway, string? PostponeReason);

public record StandingEntry(int Position, string Code, int Played, int Won, int Drawn, int Lost, int GoalsFor, int GoalsAgainst, string Form, bool IsGuest)
{
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points => Won * 3 + Drawn;
}

public record SeasonClub(string Code, bool Licensed, string? Reason);

public record ScorerEntry(string PlayerName, string Code, int Goals);

public record DisciplineEntry(string PlayerName, string Code, int Yellow, int Red, bool Suspended);

public record LeagueWorkspace(
    string SeasonName, int MatchdayPlayed, int TotalMatchdays,
    IReadOnlyList<MatchRecord> Matches,
    IReadOnlyList<StandingEntry> Standings,
    IReadOnlyList<SeasonClub> SeasonClubs,
    IReadOnlyList<ScorerEntry> Scorers,
    IReadOnlyList<DisciplineEntry> Discipline);

public interface ILeagueManagerWorkspaceService
{
    Task<LeagueWorkspace> GetWorkspaceAsync(CancellationToken ct = default);
}
