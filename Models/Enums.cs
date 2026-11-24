namespace FERW.Models;

public enum PlayerCategory
{
    MensProfessional,
    WomensProfessional,
    Amateur
}

public enum Position
{
    Goalkeeper,
    Defender,
    Midfielder,
    Forward
}

public enum PlayerStatus
{
    Active,
    Injured,
    Suspended
}

public enum RegistrationStatus
{
    Pending,
    Approved,
    Rejected
}

public enum DocumentType
{
    Contract,
    MedicalReport,
    Insurance
}

public enum RegistrationType
{
    New,
    Renewal
}

public static class EnumLabels
{
    public static string Label(this PlayerCategory c) => c switch
    {
        PlayerCategory.MensProfessional => "Men's professional",
        PlayerCategory.WomensProfessional => "Women's professional",
        PlayerCategory.Amateur => "Amateur",
        _ => c.ToString()
    };

    public static string Label(this DocumentType d) => d switch
    {
        DocumentType.Contract => "Signed contract",
        DocumentType.MedicalReport => "Medical report",
        DocumentType.Insurance => "Proof of insurance",
        _ => d.ToString()
    };

    public static string Label(this RegistrationType t) => t switch
    {
        RegistrationType.New => "New registration",
        RegistrationType.Renewal => "Renewal",
        _ => t.ToString()
    };
}
