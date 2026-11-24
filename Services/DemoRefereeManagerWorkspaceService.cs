namespace FERW.Services;

// Sample data until a database and real assignment pipeline exist.
public class DemoRefereeManagerWorkspaceService : IRefereeManagerWorkspaceService
{
    public Task<RefereeManagerWorkspace> GetWorkspaceAsync(CancellationToken ct = default)
    {
        var referees = new List<RefereeRecord>
        {
            new("r1", "Eric Nkusi", "FIFA-listed", "Active", 11, 34, 3, "Matchday 9 · 29 Nov", "Mar 2019", "+250 788 123 456", "No complaints on file",
                new[] { "Rayon Sports 2–1 Police FC · Matchday 8", "Gorilla FC 1–3 Rayon Sports · Matchday 7", "Rayon Sports 1–1 APR FC · Matchday 6" }),
            new("r2", "Marie Uwase", "FIFA-listed", "Active", 10, 29, 1, "Matchday 9 · 28 Nov", "Jul 2018", "+250 788 234 567", "No complaints on file",
                new[] { "Mukura VS 1–0 APR FC · Matchday 8", "Etincelles FC 2–2 Marines FC · Matchday 6" }),
            new("r3", "Jean Bosco", "National", "Unavailable", 9, 25, 2, "Unavailable until 2 Dec", "Jan 2021", "+250 788 345 678", "No complaints on file",
                new[] { "Etincelles FC v Marines FC · Matchday 8 (result pending)" }),
            new("r4", "Divine Ishimwe", "National", "Active", 8, 22, 1, "Matchday 9 · 28 Nov", "May 2020", "+250 788 456 789", "No complaints on file",
                new[] { "AS Kigali 1–1 Kiyovu SC · Matchday 8" }),
            new("r5", "Alice Mukamana", "National", "Active", 9, 27, 0, "Available", "Sep 2021", "+250 788 567 890", "No complaints on file",
                new[] { "Bugesera FC 0–0 Gicumbi FC · Matchday 7" }),
            new("r6", "Fabrice Niyonshuti", "Regional", "Active", 4, 11, 0, "Available", "Feb 2024", "+250 788 678 901", "New this season",
                new[] { "Mukura VS 2–0 Heroes FC · Matchday 5" }),
            new("r7", "Grace Umutoni", "Regional", "Pending", 0, 0, 0, "Application pending", "Applying", "+250 788 789 012", "Application under review — missing medical clearance",
                Array.Empty<string>()),
            new("r8", "Patrick Byusa", "National", "Active", 7, 19, 1, "Available", "Nov 2019", "+250 788 890 123", "No complaints on file",
                new[] { "Vision Generation 1–2 Musanze FC · Matchday 8" }),
            new("r9", "Olive Mutesi", "Regional", "Active", 3, 8, 0, "Available", "Jan 2025", "+250 788 901 234", "New this season",
                new[] { "Heroes FC 0–1 Bugesera FC · Matchday 4" }),
            new("r10", "Samuel Rwigema", "National", "Active", 6, 17, 0, "Available", "Jun 2022", "+250 788 012 345", "No complaints on file",
                new[] { "Vision Generation 0–0 Heroes FC · Matchday 7" }),
        };

        var actionItems = new List<ActionItem>
        {
            new("Matchday 9 — APR FC vs Rayon Sports", "Sun 29 Nov, 16:00 · no referee assigned yet", "Needs referee", "staff-status--warn"),
            new("Theogene Habimana — application", "Submitted 2 days ago · documents complete", "Pending", "staff-status--pending"),
            new("Grace Umutoni — application", "Submitted yesterday · missing medical clearance", "Incomplete", "staff-status--warn"),
            new("Jean Bosco — marked unavailable", "Unavailable 28 Nov–2 Dec · travel", "Noted", "staff-status--scheduled"),
        };

        var matchdayLabels = new List<string> { "Matchday 8 (21–22 Nov)", "Matchday 9 (28–29 Nov)", "Matchday 10 (5–6 Dec)" };

        var availability = new List<AvailabilityRow>
        {
            new("r1", new[] { "assigned", "assigned", "available" }),
            new("r2", new[] { "available", "assigned", "available" }),
            new("r3", new[] { "assigned", "unavailable", "unavailable" }),
            new("r4", new[] { "available", "assigned", "available" }),
            new("r5", new[] { "available", "available", "available" }),
            new("r6", new[] { "available", "available", "assigned" }),
            new("r8", new[] { "available", "available", "available" }),
            new("r9", new[] { "available", "available", "available" }),
            new("r10", new[] { "available", "available", "available" }),
        };

        var workspace = new RefereeManagerWorkspace(
            Referees: referees,
            ActionItems: actionItems,
            Availability: availability,
            MatchdayLabels: matchdayLabels,
            UnassignedMatchLabel: "Matchday 9 — APR FC vs Rayon Sports · Sun 29 Nov, 16:00");

        return Task.FromResult(workspace);
    }
}
