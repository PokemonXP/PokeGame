using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddVarietyMoves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VarietyMoves",
                schema: "Pokemon",
                columns: table => new
                {
                    VarietyId = table.Column<int>(type: "integer", nullable: false),
                    MoveId = table.Column<int>(type: "integer", nullable: false),
                    VarietyUid = table.Column<Guid>(type: "uuid", nullable: false),
                    MoveUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarietyMoves", x => new { x.VarietyId, x.MoveId });
                    table.ForeignKey(
                        name: "FK_VarietyMoves_Moves_MoveId",
                        column: x => x.MoveId,
                        principalSchema: "Pokemon",
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VarietyMoves_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "Pokemon",
                        principalTable: "Varieties",
                        principalColumn: "VarietyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_Level",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_MoveId",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_MoveUid",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "MoveUid");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_VarietyUid",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "VarietyUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VarietyMoves",
                schema: "Pokemon");
        }
    }
}
