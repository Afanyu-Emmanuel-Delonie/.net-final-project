using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Admin.Licensing;

public class IndexModel : PageModel
{
    private readonly IAdminWorkspaceService _workspace;

    public IndexModel(IAdminWorkspaceService workspace) => _workspace = workspace;

    public AdminWorkspace Workspace { get; private set; } = default!;
    public ClubLicense? SelectedClub { get; private set; }

    public static readonly string[] StatusOptions = { "Pending", "Licensed", "Licensed with fine", "Rejected" };

    public async Task OnGetAsync(string? id, CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
        SelectedClub = (id is null ? null : Workspace.Licenses.FirstOrDefault(c => c.Id == id))
            ?? Workspace.Licenses.FirstOrDefault();
    }

    public static string BadgeClass(string status) => status switch
    {
        "Licensed" => "staff-status--licensed",
        "Licensed with fine" => "staff-status--licensed-fine",
        "Rejected" => "staff-status--rejected",
        _ => "staff-status--pending",
    };
}
