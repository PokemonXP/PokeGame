using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddPokemonIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Experience",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Experience");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Friendship",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Friendship");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pokemon_Experience",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_Friendship",
                schema: "Pokemon",
                table: "Pokemon");
        }
    }
}
