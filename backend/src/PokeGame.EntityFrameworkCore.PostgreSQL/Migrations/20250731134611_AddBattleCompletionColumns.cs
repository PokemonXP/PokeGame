using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddBattleCompletionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletedBy",
                schema: "Pokemon",
                table: "Battles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                schema: "Pokemon",
                table: "Battles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                schema: "Pokemon",
                table: "Battles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CompletedBy",
                schema: "Pokemon",
                table: "Battles",
                column: "CompletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CompletedOn",
                schema: "Pokemon",
                table: "Battles",
                column: "CompletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Resolution",
                schema: "Pokemon",
                table: "Battles",
                column: "Resolution");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Battles_CompletedBy",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropIndex(
                name: "IX_Battles_CompletedOn",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropIndex(
                name: "IX_Battles_Resolution",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "CompletedBy",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "CompletedOn",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "Resolution",
                schema: "Pokemon",
                table: "Battles");
        }
    }
}
