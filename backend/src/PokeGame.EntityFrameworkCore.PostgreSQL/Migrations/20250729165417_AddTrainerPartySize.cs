using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerPartySize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PartySize",
                schema: "Pokemon",
                table: "Trainers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_PartySize",
                schema: "Pokemon",
                table: "Trainers",
                column: "PartySize");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trainers_PartySize",
                schema: "Pokemon",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "PartySize",
                schema: "Pokemon",
                table: "Trainers");
        }
    }
}
