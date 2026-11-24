namespace FERW.Models;

public class PlayerRegistration
{
    public int Id { get; set; }

    public int PlayerId { get; set; }
    public Player Player { get; set; } = default!;

    // Club submitting the registration. Becomes a foreign key once the club module adds the Club table.
    public int ClubId { get; set; }

    public int RegistrationWindowId { get; set; }
    public RegistrationWindow RegistrationWindow { get; set; } = default!;

    public RegistrationType RegistrationType { get; set; }
    public int? PreferredShirtNumber { get; set; }
    public DateOnly ContractStartDate { get; set; }
    public DateOnly ContractEndDate { get; set; }

    // Foreign-player registration fee recorded at submission (FR2.4).
    public int? ForeignFeeRwf { get; set; }

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
    public DateTime SubmittedAt { get; set; }
    public string SubmittedBy { get; set; } = "";
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public string? Remarks { get; set; }

    public ICollection<PlayerDocument> Documents { get; set; } = new List<PlayerDocument>();
}
