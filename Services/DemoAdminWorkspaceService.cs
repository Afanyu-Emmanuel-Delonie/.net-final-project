namespace FERW.Services;

// Sample data until a database and real approval workflow exist. Club names are real, the rest is made up.
public class DemoAdminWorkspaceService : IAdminWorkspaceService
{
    public Task<AdminWorkspace> GetWorkspaceAsync(CancellationToken ct = default)
    {
        var registrations = new List<PlayerRegistration>
        {
            new("p1", "Eric Mugisha", "APR FC", "Men's professional", "19 Nov 2026", "3 days", "Pending",
                "14 Mar 2001", "Midfielder", false, true, true, true, null),
            new("p2", "Divine Uwamahoro", "Mukura VS", "Women's professional", "21 Nov 2026", "1 day", "Pending",
                "2 Jul 2003", "Forward", false, true, true, false, null),
            new("p3", "Didier Ouedraogo", "Rayon Sports", "Men's professional", "18 Nov 2026", "4 days", "Pending",
                "9 Jan 1999", "Striker", true, true, true, true, false),
            new("p4", "Claude Bizimana", "Police FC", "Men's professional", "20 Nov 2026", "2 days", "Pending",
                "30 Oct 2002", "Defender", false, true, false, true, null),
            new("p5", "Aline Umutoni", "Kiyovu SC", "Women's professional", "17 Nov 2026", "5 days", "Pending",
                "5 Feb 2004", "Goalkeeper", false, true, true, true, null),
            new("p6", "Jonas Nshimiyimana", "Marines FC", "Amateur", "22 Nov 2026", "0 days", "Pending",
                "11 Nov 2000", "Midfielder", false, false, true, true, null),
        };

        var transfers = new List<TransferRequest>
        {
            new("t1", "Samuel Nkurunziza", "Gicumbi FC", "Kiyovu SC", "Transfer", "20 Nov 2026", "Pending",
                true, 4, 1, null),
            new("t2", "Patrick Habimana", "Marines FC", "APR FC", "Loan, 8 months", "21 Nov 2026", "Pending",
                true, 6, 2, 8),
            new("t3", "Eric Twagirayezu", "Police FC", "Rayon Sports", "Loan, 14 months", "20 Nov 2026", "Pending",
                false, 2, 1, 14),
        };

        var licenses = new List<ClubLicense>
        {
            new("c1", "Marines FC", "2026/27", "10 Nov 2026", "Licensed", null, null, true, true, true, true),
            new("c2", "Etincelles FC", "2026/27", "17 Nov 2026", "Pending", null, null, true, true, false, true),
            new("c3", "APR FC", "2026/27", "8 Nov 2026", "Licensed", null, null, true, true, true, true),
            new("c4", "Kiyovu SC", "2026/27", "12 Nov 2026", "Licensed with fine", 500000, null, true, true, true, false),
            new("c5", "Mukura VS", "2026/27", "19 Nov 2026", "Pending", null, null, true, false, true, true),
        };

        var windows = new List<RegistrationWindow>
        {
            new("w1", "Men's professional", "1 Oct 2026", "15 Dec 2026", "Open"),
            new("w2", "Women's professional", "1 Oct 2026", "20 Dec 2026", "Open"),
            new("w3", "Amateur", "1 Sep 2026", "1 Nov 2026", "Closed"),
        };

        var rules = new List<LeagueRule>
        {
            new("r1", "Foreign players on the pitch", "Maximum number of foreign-registered players a club may field at once.", 8),
            new("r2", "Minimum under-21 players per match", "Every matchday squad must include at least this many under-21 players.", 3),
            new("r3", "Maximum loans per club", "A club may have at most this many players out on loan at once.", 6),
            new("r4", "Maximum loans from the same club", "At most this many of those loans may come from a single club.", 3),
            new("r5", "Maximum loan duration (months)", "A loan may not run longer than this many months.", 12),
            new("r6", "Foreign-player registration fee (RWF)", "Charged once when a foreign player is registered.", 150000),
        };

        var users = new List<AdminUser>
        {
            new("u1", "Jean Claude Niyonzima", "jc.niyonzima@ferwafa.rw", "Federation admin", null, "Active"),
            new("u2", "Alice Uwase", "alice.uwase@rpl.rw", "League manager", null, "Active"),
            new("u3", "Eric Habyarimana", "eric.h@aprfc.rw", "Club manager", "APR FC", "Active"),
            new("u4", "Solange Mukamana", "solange.m@rayonsports.rw", "Club manager", "Rayon Sports", "Invited"),
            new("u5", "Patrick Ndayishimiye", "patrick.n@ferwafa.rw", "Referee", null, "Active"),
        };

        var summary = new AdminSummary(
            PendingRegistrations: registrations.Count(r => r.Status == "Pending"),
            OldestRegistrationWait: 3,
            PendingTransfers: transfers.Count(t => t.Status == "Pending"),
            OldestTransferWait: 2,
            PendingLicenses: licenses.Count(c => c.Status == "Pending"),
            OldestLicenseWait: 5,
            LicensedClubs: licenses.Count(c => c.Status is "Licensed" or "Licensed with fine"),
            TotalClubs: 18);

        return Task.FromResult(new AdminWorkspace(summary, registrations, transfers, licenses, windows, rules, users));
    }
}
