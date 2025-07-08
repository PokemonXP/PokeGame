using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicalMachineColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MoveId",
                schema: "Pokemon",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MoveUid",
                schema: "Pokemon",
                table: "Items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_MoveId",
                schema: "Pokemon",
                table: "Items",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_MoveUid",
                schema: "Pokemon",
                table: "Items",
                column: "MoveUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Moves_MoveId",
                schema: "Pokemon",
                table: "Items",
                column: "MoveId",
                principalSchema: "Pokemon",
                principalTable: "Moves",
                principalColumn: "MoveId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Moves_MoveId",
                schema: "Pokemon",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_MoveId",
                schema: "Pokemon",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_MoveUid",
                schema: "Pokemon",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MoveId",
                schema: "Pokemon",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MoveUid",
                schema: "Pokemon",
                table: "Items");
        }
    }
}
