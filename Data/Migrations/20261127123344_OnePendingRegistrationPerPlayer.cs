using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FERW.Data.Migrations
{
    /// <inheritdoc />
    public partial class OnePendingRegistrationPerPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerRegistrations_PlayerId",
                table: "PlayerRegistrations");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRegistrations_PlayerId_Pending",
                table: "PlayerRegistrations",
                column: "PlayerId",
                unique: true,
                filter: "[Status] = 'Pending'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerRegistrations_PlayerId_Pending",
                table: "PlayerRegistrations");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRegistrations_PlayerId",
                table: "PlayerRegistrations",
                column: "PlayerId");
        }
    }
}
