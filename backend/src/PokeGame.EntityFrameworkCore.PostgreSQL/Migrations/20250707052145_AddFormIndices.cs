using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddFormIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Forms_ExperienceYield",
                schema: "Pokemon",
                table: "Forms",
                column: "ExperienceYield");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Height",
                schema: "Pokemon",
                table: "Forms",
                column: "Height");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Weight",
                schema: "Pokemon",
                table: "Forms",
                column: "Weight");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Forms_ExperienceYield",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Forms_Height",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Forms_Weight",
                schema: "Pokemon",
                table: "Forms");
        }
    }
}
