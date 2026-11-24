using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FERW.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPlayerRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeagueRules",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueRules", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NationalIdOrPassport = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    IsForeign = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ClubId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationWindows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationWindows", x => x.Id);
                    table.CheckConstraint("CK_RegistrationWindow_Dates", "[EndDate] >= [StartDate]");
                });

            migrationBuilder.CreateTable(
                name: "PlayerRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    RegistrationWindowId = table.Column<int>(type: "int", nullable: false),
                    RegistrationType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    PreferredShirtNumber = table.Column<int>(type: "int", nullable: true),
                    ContractStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ContractEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ForeignFeeRwf = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRegistrations", x => x.Id);
                    table.CheckConstraint("CK_PlayerRegistration_Contract", "[ContractEndDate] > [ContractStartDate]");
                    table.CheckConstraint("CK_PlayerRegistration_Shirt", "[PreferredShirtNumber] IS NULL OR [PreferredShirtNumber] BETWEEN 1 AND 99");
                    table.ForeignKey(
                        name: "FK_PlayerRegistrations_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerRegistrations_RegistrationWindows_RegistrationWindowId",
                        column: x => x.RegistrationWindowId,
                        principalTable: "RegistrationWindows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerRegistrationId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    StoredPath = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerDocuments_PlayerRegistrations_PlayerRegistrationId",
                        column: x => x.PlayerRegistrationId,
                        principalTable: "PlayerRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LeagueRules",
                columns: new[] { "Key", "Description", "Value" },
                values: new object[] { "ForeignPlayerFee", "Foreign-player registration fee (RWF), charged once at registration.", 150000 });

            migrationBuilder.InsertData(
                table: "RegistrationWindows",
                columns: new[] { "Id", "Category", "EndDate", "StartDate" },
                values: new object[,]
                {
                    { 1, "MensProfessional", new DateOnly(2026, 12, 15), new DateOnly(2026, 10, 1) },
                    { 2, "WomensProfessional", new DateOnly(2026, 12, 20), new DateOnly(2026, 10, 1) },
                    { 3, "Amateur", new DateOnly(2026, 11, 1), new DateOnly(2026, 9, 1) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDocuments_PlayerRegistrationId_Type",
                table: "PlayerDocuments",
                columns: new[] { "PlayerRegistrationId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRegistrations_ClubId",
                table: "PlayerRegistrations",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRegistrations_PlayerId",
                table: "PlayerRegistrations",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRegistrations_RegistrationWindowId",
                table: "PlayerRegistrations",
                column: "RegistrationWindowId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_LicenseNumber",
                table: "Players",
                column: "LicenseNumber",
                unique: true,
                filter: "[LicenseNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Players_NationalIdOrPassport",
                table: "Players",
                column: "NationalIdOrPassport",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeagueRules");

            migrationBuilder.DropTable(
                name: "PlayerDocuments");

            migrationBuilder.DropTable(
                name: "PlayerRegistrations");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "RegistrationWindows");
        }
    }
}
