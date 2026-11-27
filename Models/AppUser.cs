using Microsoft.AspNetCore.Identity;

namespace FERW.Models;

// A signed-in FFMS user. Club managers carry the club they act for; federation staff have no club.
public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = "";

    // Plain id until the club module adds the Club table and its foreign key.
    public int? ClubId { get; set; }
    public string? ClubName { get; set; }
}
