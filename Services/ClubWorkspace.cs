namespace FERW.Services;

public record ClubProfile(string Code, string Name, string City, string Stadium, int Founded);

public record LicenseDocument(string Name, string Status, string Note);

public record PlayerRecord(
    string Id,
    string Name,
    string Position,
    string Nationality,
    bool Foreign,
    bool U21,
    string Status,
    DateOnly ContractEnd,
    int Appearances,
    int Goals,
    int YellowCards,
    int RedCards,
    string? LicenseNumber,
    DateOnly RegisteredOn);

public record ClubTransferRequest(
    string Id,
    string PlayerName,
    string Movement,
    string OtherClub,
    string Status,
    string Direction);

public record ActionItem(string Title, string Detail, string BadgeText, string BadgeClass);

public record ResultEntry(string OpponentCode, string Score, bool Home, string Matchday);

public record ClubWorkspace(
    ClubProfile Profile,
    string LicenseStatus,
    string? LicenseFineNote,
    IReadOnlyList<LicenseDocument> Documents,
    IReadOnlyList<PlayerRecord> Players,
    IReadOnlyList<ClubTransferRequest> Transfers,
    IReadOnlyList<ActionItem> ActionItems,
    string NextOpponentCode,
    DateOnly NextMatchDate,
    TimeOnly NextMatchKickoff,
    string NextMatchStadium,
    DateTime SquadDeadline,
    int LeaguePosition,
    int LeaguePositionTotal,
    int LeaguePoints,
    int MatchesPlayed,
    string FormGuide,
    IReadOnlyList<ResultEntry> RecentResults,
    int LoanPlayersOut,
    int LoanLimit,
    string SameClubLoanName,
    int SameClubLoanCount,
    int SameClubLoanLimit,
    string RegistrationWindowName,
    DateOnly RegistrationWindowCloses)
{
    public const int MaxForeignOnPitch = 8;
    public const int MinU21OnPitch = 3;
    public const int SquadStarters = 11;
}

public interface IClubManagerWorkspaceService
{
    Task<ClubWorkspace> GetWorkspaceAsync(CancellationToken ct = default);
}
