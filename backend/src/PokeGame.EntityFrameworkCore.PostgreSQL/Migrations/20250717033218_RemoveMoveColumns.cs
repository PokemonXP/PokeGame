using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMoveColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Moves_StatusCondition",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "AccuracyChange",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "AttackChange",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "CriticalChange",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "DefenseChange",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "EvasionChange",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "SpecialAttackChange",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "SpecialDefenseChange",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "SpeedChange",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "StatusChance",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "StatusCondition",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "VolatileConditions",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.AlterColumn<byte>(
                name: "PowerPoints",
                schema: "Pokemon",
                table: "Moves",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<byte>(
                name: "Power",
                schema: "Pokemon",
                table: "Moves",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<byte>(
                name: "Accuracy",
                schema: "Pokemon",
                table: "Moves",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PowerPoints",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "Power",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Accuracy",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccuracyChange",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttackChange",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CriticalChange",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefenseChange",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EvasionChange",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpecialAttackChange",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpecialDefenseChange",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpeedChange",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusChance",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StatusCondition",
                schema: "Pokemon",
                table: "Moves",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VolatileConditions",
                schema: "Pokemon",
                table: "Moves",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_StatusCondition",
                schema: "Pokemon",
                table: "Moves",
                column: "StatusCondition");
        }
    }
}
