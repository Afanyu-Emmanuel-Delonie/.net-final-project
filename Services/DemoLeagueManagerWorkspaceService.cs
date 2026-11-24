namespace FERW.Services;

// Sample data until a database and real scheduling/results pipeline exist.
public class DemoLeagueManagerWorkspaceService : ILeagueManagerWorkspaceService
{
    public Task<LeagueWorkspace> GetWorkspaceAsync(CancellationToken ct = default)
    {
        var matches = new List<MatchRecord>
        {
            new("m1", "Round 1", 8, "RAY", "POL", "Kigali Pelé Stadium",
                new DateOnly(2026, 11, 21), new TimeOnly(15, 0), "Sat 21 Nov, 15:00", "Finished", "2–1",
                "Eric Nkusi", true, true, null),
            new("m2", "Round 1", 8, "MUK", "APR", "Huye Stadium",
                new DateOnly(2026, 11, 21), new TimeOnly(15, 0), "Sat 21 Nov, 15:00", "In play", "1–0",
                "Marie Uwase", true, true, null),
            new("m3", "Round 1", 8, "ETI", "MAR", "Umuganda Stadium",
                new DateOnly(2026, 11, 22), new TimeOnly(15, 0), "Sun 22 Nov, 15:00", "Result pending", null,
                "Jean Bosco", true, false, null),
            new("m4", "Round 1", 8, "ASK", "KIY", "Kigali Pelé Stadium",
                new DateOnly(2026, 11, 22), new TimeOnly(18, 0), "Sun 22 Nov, 18:00", "Scheduled", null,
                "Divine Ishimwe", true, true, null),
            new("m5", "Round 1", 8, "GIC", "BUG", "Huye Stadium",
                null, null, "Postponed", "Postponed", null,
                null, false, false, "Pitch unplayable after heavy rain; rescheduled date to follow."),
        };

        var standings = new List<StandingEntry>
        {
            new(1, "MAR", 8, 5, 2, 1, 15, 7, "WWDWL", false),
            new(2, "APR", 8, 5, 1, 2, 13, 6, "WDWWL", false),
            new(3, "RAY", 8, 4, 3, 1, 11, 6, "DWWDW", false),
            new(4, "POL", 8, 4, 2, 2, 10, 7, "WLWDW", false),
            new(5, "KIY", 8, 3, 3, 2, 9, 7, "DDWLW", false),
            new(6, "MUK", 8, 3, 2, 3, 8, 8, "LWDWL", false),
            new(7, "ETI", 8, 2, 3, 3, 8, 9, "DLWDL", false),
            new(8, "ASK", 8, 2, 2, 4, 7, 9, "LDLWD", true),
        };

        var seasonClubs = new List<SeasonClub>
        {
            new("MAR", true, null), new("APR", true, null), new("RAY", true, null),
            new("POL", true, null), new("KIY", true, null), new("MUK", true, null),
            new("ETI", true, null),
            new("GIC", false, "License rejected — stadium inspection report missing"),
            new("BUG", true, null),
            new("ASK", false, "License still pending with Federation Admin"),
        };

        var scorers = new List<ScorerEntry>
        {
            new("Eric Mugisha", "APR", 9),
            new("Samuel Niyonzima", "RAY", 8),
            new("Didier Ouedraogo", "MAR", 7),
            new("Claude Bizimana", "POL", 6),
        };

        var discipline = new List<DisciplineEntry>
        {
            new("Jonas Nshimiyimana", "MAR", 4, 0, false),
            new("Patrick Habimana", "MUK", 2, 1, true),
            new("Eric Twagirayezu", "POL", 5, 0, false),
        };

        return Task.FromResult(new LeagueWorkspace(
            SeasonName: "2026/27",
            MatchdayPlayed: 8,
            TotalMatchdays: 15,
            Matches: matches,
            Standings: standings,
            SeasonClubs: seasonClubs,
            Scorers: scorers,
            Discipline: discipline));
    }
}
