using FERW.Models;
using FERW.Services.Players;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Admin.Windows;

public class IndexModel : PageModel
{
    private readonly IRegistrationWindowService _windows;

    public IndexModel(IRegistrationWindowService windows) => _windows = windows;

    public IReadOnlyList<WindowRow> Windows { get; private set; } = Array.Empty<WindowRow>();

    public static IReadOnlyList<PlayerCategory> Categories { get; } = Enum.GetValues<PlayerCategory>();

    [TempData]
    public string? StatusMessage { get; set; }
    [TempData]
    public bool StatusIsError { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        Windows = await _windows.GetWindowsAsync(ct);
    }

    public async Task<IActionResult> OnPostSaveAsync(int? id, PlayerCategory category, DateOnly? start, DateOnly? end, CancellationToken ct)
    {
        var result = start is null || end is null
            ? ReviewResult.Fail("Enter both a start date and an end date.")
            : await _windows.SaveAsync(id, category, start.Value, end.Value, ct);

        StatusMessage = result.Message;
        StatusIsError = !result.Succeeded;
        return RedirectToPage();
    }
}
