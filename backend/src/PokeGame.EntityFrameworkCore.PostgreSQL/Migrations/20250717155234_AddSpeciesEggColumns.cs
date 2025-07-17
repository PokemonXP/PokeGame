using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddSpeciesEggColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "EggCycles",
                schema: "Pokemon",
                table: "Species",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_EggCycles",
                schema: "Pokemon",
                table: "Species",
                column: "EggCycles");

            migrationBuilder.CreateIndex(
                name: "IX_Species_PrimaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                column: "PrimaryEggGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Species_SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                column: "SecondaryEggGroup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Species_EggCycles",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_PrimaryEggGroup",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "EggCycles",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "PrimaryEggGroup",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species");
        }
    }
}
