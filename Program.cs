using FERW.Data;
using FERW.Models;
using FERW.Services;
using FERW.Services.Players;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    // Player registration acts for the signed-in user's club; the admin area is FERWAFA staff only.
    options.Conventions.AuthorizeFolder("/Players", AppPolicies.ClubManager);
    options.Conventions.AuthorizeFolder("/Admin", AppPolicies.FederationAdmin);
});

// The LocalDB connection string lives in appsettings.Development.json; other environments must supply one.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<AppUser, IdentityRole>(o =>
    {
        o.User.RequireUniqueEmail = true;
        o.Password.RequiredLength = 10;
        o.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddClaimsPrincipalFactory<AppUserClaimsFactory>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login";
    o.AccessDeniedPath = "/Account/Login";
});
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy(AppPolicies.ClubManager, p => p.RequireRole(AppRoles.ClubManager).RequireClaim(AppClaims.ClubId));
    o.AddPolicy(AppPolicies.FederationAdmin, p => p.RequireRole(AppRoles.FederationAdmin));
});
builder.Services.AddHttpContextAccessor();

// Registration uploads: 3 documents of up to 5 MB plus a photo.
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 20 * 1024 * 1024);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<LeagueTime>();
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
builder.Services.AddHostedService<UploadCleanupService>();
builder.Services.AddScoped<ICurrentClubAccessor, ClaimsCurrentClubAccessor>();
builder.Services.AddScoped<IPlayerRegistrationService, PlayerRegistrationService>();
builder.Services.AddScoped<IRegistrationReviewService, RegistrationReviewService>();
builder.Services.AddScoped<IRegistrationWindowService, RegistrationWindowService>();

// Admin workspace: registrations and windows from the database, the other sections still demo data.
builder.Services.AddScoped<DemoAdminWorkspaceService>();
builder.Services.AddScoped<IAdminWorkspaceService, DbAdminWorkspaceService>();

// Demo data services used by pages that are not on the database yet.
builder.Services.AddScoped<ILeagueSnapshotService, DemoLeagueSnapshotService>();
builder.Services.AddScoped<ILeagueManagerWorkspaceService, DemoLeagueManagerWorkspaceService>();
builder.Services.AddScoped<IClubManagerWorkspaceService, DemoClubManagerWorkspaceService>();
builder.Services.AddScoped<IRefereeManagerWorkspaceService, DemoRefereeManagerWorkspaceService>();

var app = builder.Build();

// Bring the database schema up to date before serving requests, so pages never hit missing tables.
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
    if (app.Environment.IsDevelopment())
        await DevUserSeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
