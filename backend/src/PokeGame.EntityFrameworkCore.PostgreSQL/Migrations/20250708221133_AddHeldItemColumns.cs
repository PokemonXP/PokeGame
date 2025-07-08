using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddHeldItemColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeldItemId",
                schema: "Pokemon",
                table: "Pokemon",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HeldItemUid",
                schema: "Pokemon",
                table: "Pokemon",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_HeldItemId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "HeldItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_HeldItemUid",
                schema: "Pokemon",
                table: "Pokemon",
                column: "HeldItemUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Items_HeldItemId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "HeldItemId",
                principalSchema: "Pokemon",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Items_HeldItemId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_HeldItemId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_HeldItemUid",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HeldItemId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HeldItemUid",
                schema: "Pokemon",
                table: "Pokemon");
        }
    }
}
