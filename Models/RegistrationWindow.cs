namespace FERW.Models;

public class RegistrationWindow
{
    public int Id { get; set; }
    public PlayerCategory Category { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public bool IsOpenOn(DateOnly date) => date >= StartDate && date <= EndDate;
}
