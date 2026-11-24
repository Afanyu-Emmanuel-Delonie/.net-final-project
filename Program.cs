using FERW.Data;
using FERW.Services;
using FERW.Services.Players;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registration uploads: 3 documents of up to 5 MB plus a photo.
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 20 * 1024 * 1024);

builder.Services.AddSingleton(TimeProvider.System);
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
