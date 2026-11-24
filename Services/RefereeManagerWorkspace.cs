namespace FERW.Services;

public record RefereeRecord(
    string Id,
    string Name,
    string Certification,
    string Status,
    int MatchesThisSeason,
    int YellowCards,
    int RedCards,
    string NextMatchday,
    string LicensedSince,
    string Phone,
    string Notes,
    IReadOnlyList<string> RecentMatches);

public record AvailabilityRow(string RefereeId, IReadOnlyList<string> Cells);

public record RefereeManagerWorkspace(
    IReadOnlyList<RefereeRecord> Referees,
    IReadOnlyList<ActionItem> ActionItems,
    IReadOnlyList<AvailabilityRow> Availability,
    IReadOnlyList<string> MatchdayLabels,
    string UnassignedMatchLabel);

public interface IRefereeManagerWorkspaceService
{
    Task<RefereeManagerWorkspace> GetWorkspaceAsync(CancellationToken ct = default);
}
