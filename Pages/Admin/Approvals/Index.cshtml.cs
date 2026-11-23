using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Admin.Approvals;

public class IndexModel : PageModel
{
    private readonly IAdminWorkspaceService _workspace;

    public IndexModel(IAdminWorkspaceService workspace) => _workspace = workspace;

    public AdminWorkspace Workspace { get; private set; } = default!;
    public string Tab { get; private set; } = "players";
    public string? Club { get; private set; }
    public string? Window { get; private set; }
    public string? Status { get; private set; }
    public string? Search { get; private set; }

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

    public static bool DocsComplete(PlayerRegistration r) => r.HasContract && r.HasMedical && r.HasInsurance;

    public static bool CanApprove(PlayerRegistration r) => DocsComplete(r) && (!r.IsForeign || r.FeePaid == true);

    public static bool IsLoan(TransferRequest t) => t.Kind.StartsWith("Loan", StringComparison.OrdinalIgnoreCase);

    public static bool CanApprove(TransferRequest t) =>
        t.ReleaseAccepted && (!IsLoan(t) || (t.ClubLoanCount <= 6 && t.SameClubLoanCount <= 3 && t.LoanMonths is <= 12));
}
