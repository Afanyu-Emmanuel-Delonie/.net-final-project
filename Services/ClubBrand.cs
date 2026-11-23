namespace FERW.Services;

// Signature crest colors so club badges read as distinct teams, not interchangeable placeholders.
public static class ClubBrand
{
    private static readonly Dictionary<string, string> Colors = new()
    {
        ["MAR"] = "#2563EB",
        ["APR"] = "#047857",
        ["RAY"] = "#9A3412",
        ["POL"] = "#1F2937",
        ["KIY"] = "#BE185D",
        ["MUK"] = "#059669",
        ["ETI"] = "#D97706",
        ["GIC"] = "#7C3AED",
        ["BUG"] = "#B91C1C",
        ["ASK"] = "#CA8A04",
        ["GOR"] = "#4D7C0F",
        ["VIS"] = "#0E7490",
        ["MUS"] = "#6D28D9",
        ["HER"] = "#475569",
    };

    public static string ColorFor(string shortCode) =>
        Colors.TryGetValue(shortCode, out var hex) ? hex : "#0B6E4F";
}
