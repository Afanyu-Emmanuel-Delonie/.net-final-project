using System.Security.Claims;

namespace FERW.Services;

// Tells pages which club the signed-in club manager belongs to.
public interface ICurrentClubAccessor
{
    int ClubId { get; }
    string ClubName { get; }
    string UserName { get; }
}

// Reads the club from the signed-in user's claims (added by AppUserClaimsFactory), so every request
// acts for its own user's club. Only used on pages behind the ClubManager policy, which guarantees the claims.
public class ClaimsCurrentClubAccessor : ICurrentClubAccessor
{
    public ClaimsCurrentClubAccessor(IHttpContextAccessor http)
    {
        var user = http.HttpContext?.User
            ?? throw new InvalidOperationException("No current request.");

        var clubId = user.FindFirstValue(AppClaims.ClubId);
        if (!int.TryParse(clubId, out var id))
            throw new InvalidOperationException("The signed-in user is not linked to a club.");

        ClubId = id;
        ClubName = user.FindFirstValue(AppClaims.ClubName) ?? $"Club {id}";
        UserName = user.FindFirstValue(AppClaims.DisplayName) ?? user.Identity?.Name ?? "unknown";
    }

    public int ClubId { get; }
    public string ClubName { get; }
    public string UserName { get; }
}
