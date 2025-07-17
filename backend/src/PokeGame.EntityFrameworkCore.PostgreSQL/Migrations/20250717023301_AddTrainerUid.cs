using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerUid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "Pokemon",
                table: "Trainers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUid",
                schema: "Pokemon",
                table: "Trainers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_UserUid",
                schema: "Pokemon",
                table: "Trainers",
                column: "UserUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trainers_UserUid",
                schema: "Pokemon",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "UserUid",
                schema: "Pokemon",
                table: "Trainers");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "Pokemon",
                table: "Trainers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
