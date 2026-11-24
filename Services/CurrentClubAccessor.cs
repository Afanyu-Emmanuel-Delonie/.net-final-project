namespace FERW.Services;

// Tells pages which club the signed-in club manager belongs to.
public interface ICurrentClubAccessor
{
    int ClubId { get; }
    string UserName { get; }
}

// Temporary: reads the club from configuration ("CurrentClub" section) until ASP.NET Identity
// is added. Then swap in an implementation that reads the user's ClubId; pages don't change.
public class ConfigCurrentClubAccessor : ICurrentClubAccessor
{
    public ConfigCurrentClubAccessor(IConfiguration config)
    {
        ClubId = config.GetValue<int?>("CurrentClub:ClubId")
            ?? throw new InvalidOperationException("CurrentClub:ClubId is not configured.");
        UserName = config.GetValue<string>("CurrentClub:UserName") ?? "club.manager";
    }

    public int ClubId { get; }
    public string UserName { get; }
}
