namespace FERW.Models;

public class Player
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateOnly DateOfBirth { get; set; }
    public string Nationality { get; set; } = "";
    public string NationalIdOrPassport { get; set; } = "";
    public string? Phone { get; set; }
    public Position Position { get; set; }
    public string? PhotoPath { get; set; }
    public bool IsForeign { get; set; }
    public PlayerStatus Status { get; set; } = PlayerStatus.Active;

    // Issued by the federation when a registration is approved.
    public string? LicenseNumber { get; set; }

    // Set when the player is licensed to a club. Plain id for now: the Club table belongs to the
    // club management module, which adds the foreign key when it lands.
    public int? ClubId { get; set; }

    public ICollection<PlayerRegistration> Registrations { get; set; } = new List<PlayerRegistration>();

    public string FullName => $"{FirstName} {LastName}";
}
