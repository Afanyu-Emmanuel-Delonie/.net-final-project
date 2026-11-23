namespace FERW.Services;

public record PlayerRegistration(
    string Id, string PlayerName, string Club, string Window, string Submitted, string Waiting,
    string Status, string DateOfBirth, string Position, bool IsForeign,
    bool HasContract, bool HasMedical, bool HasInsurance, bool? FeePaid);

public record TransferRequest(
    string Id, string PlayerName, string FromClub, string ToClub, string Kind, string Submitted, string Status,
    bool ReleaseAccepted, int ClubLoanCount, int SameClubLoanCount, int? LoanMonths);

public record ClubLicense(
    string Id, string Club, string Season, string Submitted, string Status, int? FineRwf, string? RejectionReason,
    bool HasGroundCert, bool HasFinancials, bool HasInsurance, bool HasInspection);

public record RegistrationWindow(string Id, string Category, string Opens, string Closes, string Status);

public record LeagueRule(string Id, string Label, string Help, int Value);

public record AdminUser(string Id, string Name, string Email, string Role, string? Club, string Status);

public record AdminSummary(
    int PendingRegistrations, int OldestRegistrationWait,
    int PendingTransfers, int OldestTransferWait,
    int PendingLicenses, int OldestLicenseWait,
    int LicensedClubs, int TotalClubs);

public record AdminWorkspace(
    AdminSummary Summary,
    IReadOnlyList<PlayerRegistration> Registrations,
    IReadOnlyList<TransferRequest> Transfers,
    IReadOnlyList<ClubLicense> Licenses,
    IReadOnlyList<RegistrationWindow> Windows,
    IReadOnlyList<LeagueRule> Rules,
    IReadOnlyList<AdminUser> Users);

public interface IAdminWorkspaceService
{
    Task<AdminWorkspace> GetWorkspaceAsync(CancellationToken ct = default);
}
