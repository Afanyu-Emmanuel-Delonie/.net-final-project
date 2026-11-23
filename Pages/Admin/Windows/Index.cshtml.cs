using FERW.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Admin.Windows;

public class IndexModel : PageModel
{
    private readonly IAdminWorkspaceService _workspace;

    public IndexModel(IAdminWorkspaceService workspace) => _workspace = workspace;

    public AdminWorkspace Workspace { get; private set; } = default!;

    public static readonly string[] Categories = { "Men's professional", "Women's professional", "Amateur" };

    public async Task OnGetAsync(CancellationToken ct)
    {
        Workspace = await _workspace.GetWorkspaceAsync(ct);
    }
}
