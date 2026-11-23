using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool Sent { get; private set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // TODO: send a real reset email once Identity is added to this project.
        Sent = true;
        return Page();
    }

    public class InputModel
    {
        [Required, EmailAddress, Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;
    }
}
