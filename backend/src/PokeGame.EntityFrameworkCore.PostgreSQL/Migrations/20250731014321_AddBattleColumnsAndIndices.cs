using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddBattleColumnsAndIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEgg",
                schema: "Pokemon",
                table: "Pokemon",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StartedBy",
                schema: "Pokemon",
                table: "Battles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedOn",
                schema: "Pokemon",
                table: "Battles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Pokemon",
                table: "BattlePokemon",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_IsEgg",
                schema: "Pokemon",
                table: "Pokemon",
                column: "IsEgg");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_StartedBy",
                schema: "Pokemon",
                table: "Battles",
                column: "StartedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_StartedOn",
                schema: "Pokemon",
                table: "Battles",
                column: "StartedOn");

            migrationBuilder.CreateIndex(
                name: "IX_BattlePokemon_IsActive",
                schema: "Pokemon",
                table: "BattlePokemon",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pokemon_IsEgg",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Battles_StartedBy",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropIndex(
                name: "IX_Battles_StartedOn",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropIndex(
                name: "IX_BattlePokemon_IsActive",
                schema: "Pokemon",
                table: "BattlePokemon");

            migrationBuilder.DropColumn(
                name: "IsEgg",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "StartedBy",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "StartedOn",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Pokemon",
                table: "BattlePokemon");
        }
    }
}
