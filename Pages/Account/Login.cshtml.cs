using System.ComponentModel.DataAnnotations;
using FERW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERW.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signIn;

    public LoginModel(SignInManager<AppUser> signIn) => _signIn = signIn;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _signIn.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
            return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : "/");

        ModelState.AddModelError(string.Empty, result.IsLockedOut
            ? "This account is locked after too many failed attempts. Try again in a few minutes."
            : "The email or password is incorrect.");
        return Page();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _signIn.SignOutAsync();
        return RedirectToPage();
    }

    public class InputModel
    {
        [Required, EmailAddress, Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
