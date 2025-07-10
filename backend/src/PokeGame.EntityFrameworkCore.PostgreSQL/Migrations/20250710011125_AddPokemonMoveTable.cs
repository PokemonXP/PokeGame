using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddPokemonMoveTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonMoves",
                schema: "Pokemon",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "integer", nullable: false),
                    MoveId = table.Column<int>(type: "integer", nullable: false),
                    PokemonUid = table.Column<Guid>(type: "uuid", nullable: false),
                    MoveUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: true),
                    CurrentPowerPoints = table.Column<int>(type: "integer", nullable: false),
                    MaximumPowerPoints = table.Column<int>(type: "integer", nullable: false),
                    ReferencePowerPoints = table.Column<int>(type: "integer", nullable: false),
                    IsMastered = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    TechnicalMachine = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonMoves", x => new { x.PokemonId, x.MoveId });
                    table.ForeignKey(
                        name: "FK_PokemonMoves_Moves_MoveId",
                        column: x => x.MoveId,
                        principalSchema: "Pokemon",
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonMoves_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalSchema: "Pokemon",
                        principalTable: "Pokemon",
                        principalColumn: "PokemonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_MoveId",
                schema: "Pokemon",
                table: "PokemonMoves",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_MoveUid",
                schema: "Pokemon",
                table: "PokemonMoves",
                column: "MoveUid");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_PokemonUid",
                schema: "Pokemon",
                table: "PokemonMoves",
                column: "PokemonUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonMoves",
                schema: "Pokemon");
        }
    }
}
