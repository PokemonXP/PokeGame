using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "CatchRate",
                schema: "Pokemon",
                table: "Species",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<byte>(
                name: "BaseFriendship",
                schema: "Pokemon",
                table: "Species",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CatchRate",
                schema: "Pokemon",
                table: "Species",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "BaseFriendship",
                schema: "Pokemon",
                table: "Species",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");
        }
    }
}
