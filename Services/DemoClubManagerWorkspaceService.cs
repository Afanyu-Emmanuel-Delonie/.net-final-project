namespace FERW.Services;

// Sample data until a database and real registration/transfer pipeline exist.
public class DemoClubManagerWorkspaceService : IClubManagerWorkspaceService
{
    public Task<ClubWorkspace> GetWorkspaceAsync(CancellationToken ct = default)
    {
        var players = new List<PlayerRecord>
        {
            new("p1", "Haruna Niyonzima", "Goalkeeper", "Rwandan", false, false, "Active",
                new DateOnly(2027, 6, 30), 8, 0, 0, 0, "FRW-2026-0101", new DateOnly(2026, 8, 28)),
            new("p2", "Elvis Kamanzi", "Goalkeeper", "Rwandan", false, false, "Active",
                new DateOnly(2026, 12, 31), 0, 0, 0, 0, "FRW-2026-0102", new DateOnly(2026, 8, 28)),
            new("p3", "Jean-Paul Mugisha", "Centre-back", "Rwandan", false, true, "Active",
                new DateOnly(2028, 6, 30), 8, 1, 2, 0, "FRW-2026-0147", new DateOnly(2026, 8, 28)),
            new("p4", "Moise Iradukunda", "Centre-back", "Rwandan", false, false, "Injured",
                new DateOnly(2027, 6, 30), 6, 0, 1, 0, "FRW-2026-0105", new DateOnly(2026, 8, 28)),
            new("p5", "Boubacar Diallo", "Centre-back", "Malian", true, false, "Active",
                new DateOnly(2027, 6, 30), 7, 0, 3, 0, "FRW-2026-0106", new DateOnly(2026, 8, 28)),
            new("p6", "Eric Ndayishimiye", "Full-back", "Rwandan", false, true, "Active",
                new DateOnly(2029, 6, 30), 7, 0, 1, 0, "FRW-2026-0109", new DateOnly(2026, 8, 28)),
            new("p7", "Yannick Bizimana", "Full-back", "Rwandan", false, false, "Suspended",
                new DateOnly(2026, 6, 30), 5, 0, 4, 1, "FRW-2026-0110", new DateOnly(2026, 8, 28)),
            new("p8", "Patrick Habimana", "Midfielder", "Rwandan", false, false, "Active",
                new DateOnly(2027, 6, 30), 8, 2, 2, 0, "FRW-2026-0112", new DateOnly(2026, 8, 28)),
            new("p9", "Christian Mugabo", "Midfielder", "Rwandan", false, true, "Active",
                new DateOnly(2028, 6, 30), 7, 1, 1, 0, "FRW-2026-0115", new DateOnly(2026, 8, 28)),
            new("p10", "Amadou Traore", "Midfielder", "Burkinabe", true, false, "Active",
                new DateOnly(2026, 12, 20), 8, 3, 1, 0, "FRW-2026-0118", new DateOnly(2026, 8, 28)),
            new("p11", "Divin Ishimwe", "Winger", "Rwandan", false, false, "Active",
                new DateOnly(2027, 6, 30), 8, 2, 0, 0, "FRW-2026-0121", new DateOnly(2026, 8, 28)),
            new("p12", "Samuel Niyomugabo", "Winger", "Rwandan", false, true, "Active",
                new DateOnly(2029, 6, 30), 6, 1, 0, 0, "FRW-2026-0124", new DateOnly(2026, 8, 28)),
            new("p13", "Kelvin Mushimiyimana", "Striker", "Rwandan", false, false, "Active",
                new DateOnly(2027, 6, 30), 8, 4, 1, 0, "FRW-2026-0127", new DateOnly(2026, 8, 28)),
            new("p14", "Mohamed Keita", "Striker", "Guinean", true, false, "Active",
                new DateOnly(2027, 6, 30), 8, 5, 0, 0, "FRW-2026-0130", new DateOnly(2026, 8, 28)),
        };

        var documents = new List<LicenseDocument>
        {
            new("Club statutes & registration", "Approved", "Approved 12 Aug 2026"),
            new("Proof of stadium ownership or lease", "Approved", "Approved 12 Aug 2026"),
            new("Stadium floodlight certificate", "Rejected", "Expired — fine applied, re-upload to clear it"),
            new("Financial guarantee", "Approved", "Approved 15 Aug 2026"),
            new("Medical staff & ambulance contract", "Pending", "Under review since 20 Nov 2026"),
        };

        var transfers = new List<ClubTransferRequest>
        {
            new("t1", "Ange Niyibizi", "Transfer in", "Free agent", "Pending FERWAFA", "Outgoing"),
            new("t2", "Yannick Bizimana", "Loan out", "Gicumbi FC", "Awaiting their response", "Outgoing"),
            new("t3", "Christian Mugabo", "Loan in request", "APR FC", "Action needed", "Incoming"),
        };

        var actionItems = new List<ActionItem>
        {
            new("Didier Uwimana — registration", "Submitted 3 days ago, awaiting FERWAFA review", "Pending", "staff-status--pending"),
            new("Loan request from APR FC", "They want Christian Mugabo on loan — respond by Fri 27 Nov", "Action needed", "staff-status--pending"),
            new("FERWAFA requested a document", "Updated proof of insurance for foreign players — due in 4 days", "Requested", "staff-status--warn"),
            new("Mid-season registration window", "Closes in 6 days — last chance to register new signings", "6 days left", "staff-status--scheduled"),
        };

        var recentResults = new List<ResultEntry>
        {
            new("POL", "2–1", true, "Matchday 8"),
            new("GOR", "3–1", false, "Matchday 7"),
            new("APR", "1–1", true, "Matchday 6"),
        };

        var workspace = new ClubWorkspace(
            Profile: new ClubProfile("RAY", "Rayon Sports FC", "Kigali", "Kigali Pelé Stadium", 1968),
            LicenseStatus: "Licensed, with fine",
            LicenseFineNote: "A fine of 500,000 RWF applies — stadium floodlight certificate expired",
            Documents: documents,
            Players: players,
            Transfers: transfers,
            ActionItems: actionItems,
            NextOpponentCode: "APR",
            NextMatchDate: new DateOnly(2026, 11, 29),
            NextMatchKickoff: new TimeOnly(16, 0),
            NextMatchStadium: "Kigali Pelé Stadium",
            SquadDeadline: new DateTime(2026, 11, 28, 16, 0, 0),
            LeaguePosition: 3,
            LeaguePositionTotal: 8,
            LeaguePoints: 15,
            MatchesPlayed: 8,
            FormGuide: "DWWDW",
            RecentResults: recentResults,
            LoanPlayersOut: 2,
            LoanLimit: 6,
            SameClubLoanName: "Gicumbi FC",
            SameClubLoanCount: 1,
            SameClubLoanLimit: 3,
            RegistrationWindowName: "Mid-season registration window",
            RegistrationWindowCloses: new DateOnly(2026, 11, 30));

        return Task.FromResult(workspace);
    }
}
