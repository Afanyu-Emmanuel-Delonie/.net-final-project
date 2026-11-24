using FERW.Models;
using Microsoft.EntityFrameworkCore;

namespace FERW.Data;

// Shared database context for all modules. Add your module's DbSets and configuration here.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RegistrationWindow> RegistrationWindows => Set<RegistrationWindow>();
    public DbSet<LeagueRule> LeagueRules => Set<LeagueRule>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerRegistration> PlayerRegistrations => Set<PlayerRegistration>();
    public DbSet<PlayerDocument> PlayerDocuments => Set<PlayerDocument>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        // Store enums as readable strings rather than integers.
        builder.Properties<PlayerCategory>().HaveConversion<string>().HaveMaxLength(32);
        builder.Properties<Position>().HaveConversion<string>().HaveMaxLength(32);
        builder.Properties<PlayerStatus>().HaveConversion<string>().HaveMaxLength(32);
        builder.Properties<RegistrationStatus>().HaveConversion<string>().HaveMaxLength(32);
        builder.Properties<DocumentType>().HaveConversion<string>().HaveMaxLength(32);
        builder.Properties<RegistrationType>().HaveConversion<string>().HaveMaxLength(32);
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<RegistrationWindow>(e =>
        {
            e.ToTable(t => t.HasCheckConstraint("CK_RegistrationWindow_Dates", "[EndDate] >= [StartDate]"));
        });

        b.Entity<LeagueRule>(e =>
        {
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(200);
        });

        b.Entity<Player>(e =>
        {
            e.Property(x => x.FirstName).HasMaxLength(60).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(60).IsRequired();
            e.Property(x => x.Nationality).HasMaxLength(60).IsRequired();
            e.Property(x => x.NationalIdOrPassport).HasMaxLength(30).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(20);
            e.Property(x => x.PhotoPath).HasMaxLength(260);
            e.Property(x => x.LicenseNumber).HasMaxLength(20);
            e.Ignore(x => x.FullName);

            e.HasIndex(x => x.NationalIdOrPassport).IsUnique();
            e.HasIndex(x => x.LicenseNumber).IsUnique().HasFilter("[LicenseNumber] IS NOT NULL");
            e.HasIndex(x => x.ClubId);
        });

        b.Entity<PlayerRegistration>(e =>
        {
            e.Property(x => x.SubmittedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.ReviewedBy).HasMaxLength(100);
            e.Property(x => x.Remarks).HasMaxLength(500);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_PlayerRegistration_Contract", "[ContractEndDate] > [ContractStartDate]");
                t.HasCheckConstraint("CK_PlayerRegistration_Shirt", "[PreferredShirtNumber] IS NULL OR [PreferredShirtNumber] BETWEEN 1 AND 99");
            });

            e.HasOne(x => x.Player).WithMany(p => p.Registrations)
                .HasForeignKey(x => x.PlayerId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.ClubId);
            e.HasOne(x => x.RegistrationWindow).WithMany()
                .HasForeignKey(x => x.RegistrationWindowId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<PlayerDocument>(e =>
        {
            e.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
            e.Property(x => x.StoredPath).HasMaxLength(400).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.PlayerRegistrationId, x.Type }).IsUnique();

            e.HasOne(x => x.PlayerRegistration).WithMany(r => r.Documents)
                .HasForeignKey(x => x.PlayerRegistrationId).OnDelete(DeleteBehavior.Cascade);
        });

        Seed(b);
    }

    private static void Seed(ModelBuilder b)
    {
        b.Entity<RegistrationWindow>().HasData(
            new RegistrationWindow { Id = 1, Category = PlayerCategory.MensProfessional, StartDate = new DateOnly(2026, 10, 1), EndDate = new DateOnly(2026, 12, 15) },
            new RegistrationWindow { Id = 2, Category = PlayerCategory.WomensProfessional, StartDate = new DateOnly(2026, 10, 1), EndDate = new DateOnly(2026, 12, 20) },
            new RegistrationWindow { Id = 3, Category = PlayerCategory.Amateur, StartDate = new DateOnly(2026, 9, 1), EndDate = new DateOnly(2026, 11, 1) });

        b.Entity<LeagueRule>().HasData(
            new LeagueRule { Key = LeagueRule.ForeignPlayerFee, Value = 150000, Description = "Foreign-player registration fee (RWF), charged once at registration." });
    }
}
