using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class PokemonChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Statistics",
                schema: "Pokemon",
                table: "Pokemon",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nature",
                schema: "Pokemon",
                table: "Pokemon",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "Characteristic",
                schema: "Pokemon",
                table: "Pokemon",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatusCondition",
                schema: "Pokemon",
                table: "Pokemon",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_StatusCondition",
                schema: "Pokemon",
                table: "Pokemon",
                column: "StatusCondition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pokemon_StatusCondition",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "Characteristic",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "StatusCondition",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.AlterColumn<string>(
                name: "Statistics",
                schema: "Pokemon",
                table: "Pokemon",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nature",
                schema: "Pokemon",
                table: "Pokemon",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8);
        }
    }
}
