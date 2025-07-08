using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Medicine",
                schema: "Pokemon",
                table: "Items",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Medicine",
                schema: "Pokemon",
                table: "Items");
        }
    }
}
