using FERW.Data;
using FERW.Services;
using FERW.Services.Players;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// The LocalDB connection string lives in appsettings.Development.json; other environments must supply one.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Registration uploads: 3 documents of up to 5 MB plus a photo.
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 20 * 1024 * 1024);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<LeagueTime>();
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
builder.Services.AddScoped<ICurrentClubAccessor, ConfigCurrentClubAccessor>();
builder.Services.AddScoped<IPlayerRegistrationService, PlayerRegistrationService>();

// Demo data services used by pages that are not on the database yet.
builder.Services.AddScoped<ILeagueSnapshotService, DemoLeagueSnapshotService>();
builder.Services.AddScoped<IAdminWorkspaceService, DemoAdminWorkspaceService>();
builder.Services.AddScoped<ILeagueManagerWorkspaceService, DemoLeagueManagerWorkspaceService>();
builder.Services.AddScoped<IClubManagerWorkspaceService, DemoClubManagerWorkspaceService>();
builder.Services.AddScoped<IRefereeManagerWorkspaceService, DemoRefereeManagerWorkspaceService>();

var app = builder.Build();

// Bring the database schema up to date before serving requests, so pages never hit missing tables.
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
