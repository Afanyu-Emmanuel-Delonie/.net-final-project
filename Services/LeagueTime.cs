namespace FERW.Services;

// The league's calendar time zone (configured as "League:TimeZone", an IANA id such as "Africa/Kigali").
// Window dates and displayed timestamps use this zone so results don't depend on the server's time zone.
public class LeagueTime
{
    public LeagueTime(IConfiguration config)
    {
        var id = config.GetValue<string>("League:TimeZone") ?? "Africa/Kigali";
        Zone = TimeZoneInfo.FindSystemTimeZoneById(id);
    }

    public TimeZoneInfo Zone { get; }

    public DateOnly Today(TimeProvider clock) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.GetUtcNow(), Zone).DateTime);

    // Timestamps are stored as UTC in datetime2, which comes back with Kind = Unspecified.
    public DateTime ToLeagueTime(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone);
}
