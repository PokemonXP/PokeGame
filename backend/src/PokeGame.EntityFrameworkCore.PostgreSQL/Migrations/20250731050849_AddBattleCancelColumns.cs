using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddBattleCancelColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                schema: "Pokemon",
                table: "Battles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledOn",
                schema: "Pokemon",
                table: "Battles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CancelledBy",
                schema: "Pokemon",
                table: "Battles",
                column: "CancelledBy");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CancelledOn",
                schema: "Pokemon",
                table: "Battles",
                column: "CancelledOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Battles_CancelledBy",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropIndex(
                name: "IX_Battles_CancelledOn",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "CancelledOn",
                schema: "Pokemon",
                table: "Battles");
        }
    }
}
