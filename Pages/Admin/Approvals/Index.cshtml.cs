using System.Security.Claims;
using FERW.Services;
using FERW.Services.Players;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Admin.Approvals;

public class IndexModel : PageModel
{
    private readonly IAdminWorkspaceService _workspace;
    private readonly IRegistrationReviewService _reviews;

    public IndexModel(IAdminWorkspaceService workspace, IRegistrationReviewService reviews)
    {
        _workspace = workspace;
        _reviews = reviews;
    }

    public AdminWorkspace Workspace { get; private set; } = default!;
    public string Tab { get; private set; } = "players";
    public string? Club { get; private set; }
    public string? Window { get; private set; }
    public string? Status { get; private set; }
    public string? Search { get; private set; }

    [TempData]
    public string? StatusMessage { get; set; }
    [TempData]
    public bool StatusIsError { get; set; }

    public IReadOnlyList<PlayerRegistration> FilteredRegistrations { get; private set; } = Array.Empty<PlayerRegistration>();
    public PlayerRegistration? SelectedRegistration { get; private set; }
    public TransferRequest? SelectedTransfer { get; private set; }

    public async Task OnGetAsync(string? tab, string? id, string? club, string? window, string? status, string? search, CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
        Tab = tab == "transfers" ? "transfers" : "players";
        Club = club;
        Window = window;
        Status = status;
        Search = search;

        FilteredRegistrations = Workspace.Registrations
            .Where(r => string.IsNullOrEmpty(Club) || r.Club == Club)
            .Where(r => string.IsNullOrEmpty(Window) || r.Window == Window)
            .Where(r => string.IsNullOrEmpty(Status) || r.Status == Status)
            .Where(r => string.IsNullOrEmpty(Search) || r.PlayerName.Contains(Search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        SelectedRegistration = (id is null ? null : Workspace.Registrations.FirstOrDefault(r => r.Id == id))
            ?? FilteredRegistrations.FirstOrDefault();
        SelectedTransfer = (id is null ? null : Workspace.Transfers.FirstOrDefault(t => t.Id == id))
            ?? Workspace.Transfers.FirstOrDefault();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id, CancellationToken ct) =>
        Done(id, await _reviews.ApproveAsync(id, Reviewer, ct));

    public async Task<IActionResult> OnPostRejectAsync(int id, string? reason, CancellationToken ct) =>
        Done(id, await _reviews.RejectAsync(id, Reviewer, reason, ct));

    public async Task<IActionResult> OnPostFeePaidAsync(int id, CancellationToken ct) =>
        Done(id, await _reviews.MarkFeePaidAsync(id, Reviewer, ct));

    private string Reviewer =>
        User.FindFirstValue(AppClaims.DisplayName) ?? User.Identity?.Name ?? "FERWAFA";

    private IActionResult Done(int id, ReviewResult result)
    {
        StatusMessage = result.Message;
        StatusIsError = !result.Succeeded;
        return RedirectToPage(new { tab = "players", id = id.ToString() });
    }

    public static bool IsPending(PlayerRegistration r) => r.Status == "Pending";

    public static bool DocsComplete(PlayerRegistration r) => r.HasContract && r.HasMedical && r.HasInsurance;

    public static bool CanApprove(PlayerRegistration r) => IsPending(r) && DocsComplete(r) && (!r.IsForeign || r.FeePaid == true);

    public static bool IsLoan(TransferRequest t) => t.Kind.StartsWith("Loan", StringComparison.OrdinalIgnoreCase);

    public static bool CanApprove(TransferRequest t) =>
        t.ReleaseAccepted && (!IsLoan(t) || (t.ClubLoanCount <= 6 && t.SameClubLoanCount <= 3 && t.LoanMonths is <= 12));
}
