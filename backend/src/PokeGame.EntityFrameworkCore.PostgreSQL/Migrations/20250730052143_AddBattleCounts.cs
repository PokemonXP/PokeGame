using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddBattleCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChampionCount",
                schema: "Pokemon",
                table: "Battles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpponentCount",
                schema: "Pokemon",
                table: "Battles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Battles_ChampionCount",
                schema: "Pokemon",
                table: "Battles",
                column: "ChampionCount");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_OpponentCount",
                schema: "Pokemon",
                table: "Battles",
                column: "OpponentCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Battles_ChampionCount",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropIndex(
                name: "IX_Battles_OpponentCount",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "ChampionCount",
                schema: "Pokemon",
                table: "Battles");

            migrationBuilder.DropColumn(
                name: "OpponentCount",
                schema: "Pokemon",
                table: "Battles");
        }
    }
}
