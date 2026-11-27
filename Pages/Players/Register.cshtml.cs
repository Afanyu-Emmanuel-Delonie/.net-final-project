using System.ComponentModel.DataAnnotations;
using FERW.Models;
using FERW.Services;
using FERW.Services.Players;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FERW.Pages.Players;

public class RegisterModel : PageModel
{
    private readonly IPlayerRegistrationService _registrations;
    private readonly ICurrentClubAccessor _currentClub;
    private readonly LeagueTime _leagueTime;

    public RegisterModel(IPlayerRegistrationService registrations, ICurrentClubAccessor currentClub, LeagueTime leagueTime)
    {
        _registrations = registrations;
        _currentClub = currentClub;
        _leagueTime = leagueTime;
    }

    public DateTime ToLeagueTime(DateTime utc) => _leagueTime.ToLeagueTime(utc);

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    public IReadOnlyList<OpenWindow> OpenWindows { get; private set; } = Array.Empty<OpenWindow>();
    public IReadOnlyList<ClubSubmission> Submissions { get; private set; } = Array.Empty<ClubSubmission>();

    public IEnumerable<SelectListItem> WindowOptions => OpenWindows.Select(w =>
        new SelectListItem($"{w.Category.Label()} (closes {w.EndDate:d MMM yyyy})", w.Id.ToString()));

    public IEnumerable<SelectListItem> NationalityOptions => Nationalities.All.Select(n => new SelectListItem(n, n));

    public IEnumerable<SelectListItem> PositionOptions => Enum.GetValues<Position>()
        .Select(p => new SelectListItem(p.ToString(), p.ToString()));

    public IEnumerable<SelectListItem> RegistrationTypeOptions => Enum.GetValues<RegistrationType>()
        .Select(t => new SelectListItem(t.Label(), t.ToString()));

    public async Task OnGetAsync(CancellationToken ct)
    {
        await LoadAsync(ct);
        if (OpenWindows.Count == 1)
            Input.RegistrationWindowId = OpenWindows[0].Id;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(ct);
            return Page();
        }

        var request = new RegistrationRequest(
            Input.FirstName!, Input.LastName!, Input.DateOfBirth!.Value, Input.Nationality!, Input.NationalIdOrPassport!,
            Input.Phone, Input.Position!.Value, Input.PreferredShirtNumber, Input.RegistrationType!.Value,
            Input.RegistrationWindowId!.Value, Input.ContractStartDate!.Value, Input.ContractEndDate!.Value);
        var files = new RegistrationFiles(Input.Contract, Input.MedicalReport, Input.Insurance, Input.Photo);

        var result = await _registrations.SubmitAsync(_currentClub.ClubId, _currentClub.UserName, request, files, ct);
        if (!result.Succeeded)
        {
            foreach (var (field, message) in result.Errors)
                ModelState.AddModelError(field.Length == 0 ? string.Empty : $"{nameof(Input)}.{field}", message);
            await LoadAsync(ct);
            return Page();
        }

        SuccessMessage = $"{Input.FirstName} {Input.LastName} was submitted and is awaiting FERWAFA review.";
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken ct)
    {
        OpenWindows = await _registrations.GetOpenWindowsAsync(ct);
        Submissions = await _registrations.GetClubSubmissionsAsync(_currentClub.ClubId, ct: ct);
    }

    public class InputModel
    {
        [Required, StringLength(60), Display(Name = "First name")]
        public string? FirstName { get; set; }

        [Required, StringLength(60), Display(Name = "Last name")]
        public string? LastName { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Date of birth")]
        public DateOnly? DateOfBirth { get; set; }

        [Required, Display(Name = "Nationality")]
        public string? Nationality { get; set; } = Nationalities.Local;

        [Required, StringLength(30, MinimumLength = 5), Display(Name = "National ID or passport number")]
        public string? NationalIdOrPassport { get; set; }

        [Phone, StringLength(20), Display(Name = "Phone number")]
        public string? Phone { get; set; }

        [Required, Display(Name = "Position")]
        public Position? Position { get; set; }

        [Range(1, 99), Display(Name = "Preferred shirt number")]
        public int? PreferredShirtNumber { get; set; }

        [Required, Display(Name = "Registration type")]
        public RegistrationType? RegistrationType { get; set; } = Models.RegistrationType.New;

        [Required(ErrorMessage = "Choose a registration window."), Display(Name = "Registration window")]
        public int? RegistrationWindowId { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Contract start date")]
        public DateOnly? ContractStartDate { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Contract end date")]
        public DateOnly? ContractEndDate { get; set; }

        [Required(ErrorMessage = "Signed contract is required."), Display(Name = "Signed contract")]
        public IFormFile? Contract { get; set; }

        [Required(ErrorMessage = "Medical report is required."), Display(Name = "Medical report")]
        public IFormFile? MedicalReport { get; set; }

        [Required(ErrorMessage = "Proof of insurance is required."), Display(Name = "Proof of insurance")]
        public IFormFile? Insurance { get; set; }

        [Display(Name = "Player photo")]
        public IFormFile? Photo { get; set; }
    }
}
