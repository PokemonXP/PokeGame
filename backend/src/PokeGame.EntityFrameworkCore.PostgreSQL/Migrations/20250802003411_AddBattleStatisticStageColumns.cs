using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddBattleStatisticStageColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Accuracy",
                schema: "Pokemon",
                table: "BattlePokemon",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Attack",
                schema: "Pokemon",
                table: "BattlePokemon",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Critical",
                schema: "Pokemon",
                table: "BattlePokemon",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Defense",
                schema: "Pokemon",
                table: "BattlePokemon",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Evasion",
                schema: "Pokemon",
                table: "BattlePokemon",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpecialAttack",
                schema: "Pokemon",
                table: "BattlePokemon",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpecialDefense",
                schema: "Pokemon",
                table: "BattlePokemon",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Speed",
                schema: "Pokemon",
                table: "BattlePokemon",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accuracy",
                schema: "Pokemon",
                table: "BattlePokemon");

            migrationBuilder.DropColumn(
                name: "Attack",
                schema: "Pokemon",
                table: "BattlePokemon");

            migrationBuilder.DropColumn(
                name: "Critical",
                schema: "Pokemon",
                table: "BattlePokemon");

            migrationBuilder.DropColumn(
                name: "Defense",
                schema: "Pokemon",
                table: "BattlePokemon");

            migrationBuilder.DropColumn(
                name: "Evasion",
                schema: "Pokemon",
                table: "BattlePokemon");

            migrationBuilder.DropColumn(
                name: "SpecialAttack",
                schema: "Pokemon",
                table: "BattlePokemon");

            migrationBuilder.DropColumn(
                name: "SpecialDefense",
                schema: "Pokemon",
                table: "BattlePokemon");

            migrationBuilder.DropColumn(
                name: "Speed",
                schema: "Pokemon",
                table: "BattlePokemon");
        }
    }
}
