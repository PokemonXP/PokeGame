using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddPokemonIndicesAndInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventory",
                schema: "Pokemon",
                columns: table => new
                {
                    TrainerId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    TrainerUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => new { x.TrainerId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_Inventory_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Pokemon",
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "Pokemon",
                        principalTable: "Trainers",
                        principalColumn: "TrainerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Box",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Box");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_EggCycles",
                schema: "Pokemon",
                table: "Pokemon",
                column: "EggCycles");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Experience",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Experience");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Level",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Position",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ItemId",
                schema: "Pokemon",
                table: "Inventory",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ItemUid",
                schema: "Pokemon",
                table: "Inventory",
                column: "ItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Quantity",
                schema: "Pokemon",
                table: "Inventory",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_TrainerUid",
                schema: "Pokemon",
                table: "Inventory",
                column: "TrainerUid");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_TrainerUid_ItemUid",
                schema: "Pokemon",
                table: "Inventory",
                columns: new[] { "TrainerUid", "ItemUid" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory",
                schema: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_Box",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_EggCycles",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_Experience",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_Level",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_Position",
                schema: "Pokemon",
                table: "Pokemon");
        }
    }
}
