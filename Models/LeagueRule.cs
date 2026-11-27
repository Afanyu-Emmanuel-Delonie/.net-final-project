namespace FERW.Models;

// Configurable league rules, so rule changes don't need code changes (FR6.3).
public class LeagueRule
{
    public const string ForeignPlayerFee = "ForeignPlayerFee";

    public string Key { get; set; } = "";
    public int Value { get; set; }
    public string Description { get; set; } = "";
}
