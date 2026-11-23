namespace FERW.Services;

// Sample data until the database exists. Club names are real, scores are made up.
public class DemoLeagueSnapshotService : ILeagueSnapshotService
{
    private static readonly ClubRef Marines = new("Marines FC", "MAR");
    private static readonly ClubRef Apr = new("APR FC", "APR");
    private static readonly ClubRef Rayon = new("Rayon Sports", "RAY");
    private static readonly ClubRef Police = new("Police FC", "POL");
    private static readonly ClubRef Mukura = new("Mukura VS", "MUK");
    private static readonly ClubRef Gicumbi = new("Gicumbi FC", "GIC");
    private static readonly ClubRef AsKigali = new("AS Kigali", "ASK");
    private static readonly ClubRef Kiyovu = new("Kiyovu SC", "KIY");
    private static readonly ClubRef Bugesera = new("Bugesera FC", "BUG");
    private static readonly ClubRef Etincelles = new("Etincelles FC", "ETI");
    private static readonly ClubRef Gorilla = new("Gorilla FC", "GOR");
    private static readonly ClubRef VisionGeneration = new("Vision Generation", "VIS");
    private static readonly ClubRef Musanze = new("Musanze FC", "MUS");
    private static readonly ClubRef Heroes = new("Heroes FC", "HER");

    public Task<LeagueSnapshot> GetCurrentAsync(CancellationToken ct = default)
    {
        var results = new List<ResultItem>
        {
            new(Marines, Bugesera, 2, 1, new DateTime(2026, 11, 21), "Umuganda Stadium"),
            new(Apr, Rayon, 1, 1, new DateTime(2026, 11, 21), "Kigali Pelé Stadium"),
            new(Police, Etincelles, 2, 0, new DateTime(2026, 11, 22), "Kigali Pelé Stadium"),
            new(Mukura, Gicumbi, 0, 0, new DateTime(2026, 11, 22), "Huye Stadium"),
        };

        var standings = new List<StandingRow>
        {
            new(1, Marines, 8, 5, 2, 1, 15, 7),
            new(2, Apr, 8, 5, 1, 2, 13, 6),
            new(3, Rayon, 8, 4, 3, 1, 11, 6),
            new(4, Police, 8, 4, 2, 2, 10, 7),
            new(5, Mukura, 8, 3, 3, 2, 9, 7),
            new(6, Gicumbi, 8, 3, 2, 3, 8, 8),
            new(7, Etincelles, 8, 2, 3, 3, 8, 9),
            new(8, Kiyovu, 8, 2, 2, 4, 7, 9),
            new(9, Bugesera, 8, 2, 1, 5, 7, 12),
            new(10, AsKigali, 8, 1, 3, 4, 6, 11),
            new(11, Gorilla, 8, 1, 2, 5, 5, 12),
            new(12, VisionGeneration, 8, 1, 1, 6, 4, 13),
            new(13, Musanze, 8, 1, 0, 7, 3, 15),
            new(14, Heroes, 8, 0, 1, 7, 2, 17),
        };

        var fixtures = new List<FixtureItem>
        {
            new(Rayon, Police, new DateTime(2026, 11, 28, 15, 0, 0), "Kigali Pelé Stadium"),
            new(Mukura, Apr, new DateTime(2026, 11, 28, 15, 0, 0), "Huye Stadium"),
            new(Etincelles, Marines, new DateTime(2026, 11, 29, 15, 0, 0), "Umuganda Stadium"),
            new(AsKigali, Kiyovu, new DateTime(2026, 11, 29, 18, 0, 0), "Kigali Pelé Stadium"),
        };

        return Task.FromResult(new LeagueSnapshot(
            SeasonName: "2026/27",
            MatchdayPlayed: 8,
            IsDemoData: true,
            NextMatch: fixtures[0],
            LatestResults: results,
            Standings: standings,
            UpcomingFixtures: fixtures));
    }
}