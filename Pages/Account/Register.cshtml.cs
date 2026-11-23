using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Account;

public class RegisterModel : PageModel
{
    public static readonly string[] Clubs =
    {
        "Marines FC", "APR FC", "Rayon Sports", "Police FC", "Kiyovu SC",
        "Mukura VS", "Etincelles FC", "Gicumbi FC", "Bugesera FC",
    };

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (Input.Role != "club")
        {
            ModelState.Remove($"{nameof(Input)}.{nameof(Input.Club)}");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // TODO: create a pending account request once Identity and a database are added to this project.
        ModelState.AddModelError(string.Empty, "Account requests aren't connected to FERWAFA's approval queue yet.");
        return Page();
    }

    public class InputModel
    {
        [Required, Display(Name = "Full name")]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;

        [Required, Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Club")]
        public string? Club { get; set; }

        [Required, DataType(DataType.Password), MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Confirm password")]
        [Compare(nameof(Password))]
        public string Password2 { get; set; } = string.Empty;
    }
}
