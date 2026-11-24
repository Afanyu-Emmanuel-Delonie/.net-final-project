namespace FERW.Models;

public static class Nationalities
{
    public const string Local = "Rwandan";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Local,
        "Burundian", "Ugandan", "Tanzanian", "Kenyan", "Congolese (DRC)", "South Sudanese",
        "Cameroonian", "Ghanaian", "Nigerian", "Ivorian", "Malian", "Burkinabe", "Guinean",
        "Senegalese", "Zambian", "Zimbabwean", "Malawian", "Ethiopian", "Sudanese",
        "Other"
    };

    public static bool IsForeign(string nationality) =>
        !string.Equals(nationality, Local, StringComparison.OrdinalIgnoreCase);
}
