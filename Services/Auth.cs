using System.Security.Claims;
using FERW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FERW.Services;

public static class AppRoles
{
    public const string ClubManager = "ClubManager";
    public const string FederationAdmin = "FederationAdmin";
}

public static class AppPolicies
{
    // Club manager linked to a club: may register players for that club.
    public const string ClubManager = "ClubManager";
    // FERWAFA staff: may review registrations and manage registration windows.
    public const string FederationAdmin = "FederationAdmin";
}

public static class AppClaims
{
    public const string ClubId = "ffms:club_id";
    public const string ClubName = "ffms:club_name";
    public const string DisplayName = "ffms:display_name";
}

// Puts the user's club and display name into the sign-in cookie so each request knows who it acts for.
public class AppUserClaimsFactory : UserClaimsPrincipalFactory<AppUser, IdentityRole>
{
    public AppUserClaimsFactory(UserManager<AppUser> users, RoleManager<IdentityRole> roles, IOptions<IdentityOptions> options)
        : base(users, roles, options) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim(AppClaims.DisplayName, string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName ?? "" : user.DisplayName));
        if (user.ClubId is int clubId)
        {
            identity.AddClaim(new Claim(AppClaims.ClubId, clubId.ToString()));
            if (!string.IsNullOrWhiteSpace(user.ClubName))
                identity.AddClaim(new Claim(AppClaims.ClubName, user.ClubName));
        }
        return identity;
    }
}

// Development only: creates the roles and two sign-in accounts so the team can try the protected pages.
// The password comes from configuration ("DevSeed:Password"); nothing is seeded outside Development.
public static class DevUserSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { AppRoles.ClubManager, AppRoles.FederationAdmin })
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole(role));

        var password = config["DevSeed:Password"];
        if (string.IsNullOrEmpty(password))
            return;

        var users = services.GetRequiredService<UserManager<AppUser>>();
        await EnsureUserAsync(users, "club.manager@ffms.local", "Diane Uwimana", AppRoles.ClubManager, 2, "Rayon Sports", password);
        await EnsureUserAsync(users, "admin@ffms.local", "Jean Claude Niyonzima", AppRoles.FederationAdmin, null, null, password);
    }

    private static async Task EnsureUserAsync(UserManager<AppUser> users, string email, string name, string role,
        int? clubId, string? clubName, string password)
    {
        if (await users.FindByEmailAsync(email) is not null)
            return;

        var user = new AppUser
        {
            UserName = email, Email = email, EmailConfirmed = true,
            DisplayName = name, ClubId = clubId, ClubName = clubName,
        };
        var result = await users.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Could not seed {email}: {string.Join("; ", result.Errors.Select(e => e.Description))}");
        await users.AddToRoleAsync(user, role);
    }
}
