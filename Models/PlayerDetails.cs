namespace FERW.Models;

// The player's details exactly as submitted with one registration. The Player row holds the latest
// details; each registration keeps its own copy so history doesn't change when a player is resubmitted.
public class PlayerDetails
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateOnly DateOfBirth { get; set; }
    public string Nationality { get; set; } = "";
    public string? Phone { get; set; }
    public Position Position { get; set; }
    public bool IsForeign { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
