using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnershipColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentTrainerId",
                schema: "Pokemon",
                table: "Pokemon",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentTrainerUid",
                schema: "Pokemon",
                table: "Pokemon",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MetAtLevel",
                schema: "Pokemon",
                table: "Pokemon",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetDescription",
                schema: "Pokemon",
                table: "Pokemon",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetLocation",
                schema: "Pokemon",
                table: "Pokemon",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MetOn",
                schema: "Pokemon",
                table: "Pokemon",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginalTrainerId",
                schema: "Pokemon",
                table: "Pokemon",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalTrainerUid",
                schema: "Pokemon",
                table: "Pokemon",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PokeBallId",
                schema: "Pokemon",
                table: "Pokemon",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PokeBallUid",
                schema: "Pokemon",
                table: "Pokemon",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_CurrentTrainerId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "CurrentTrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_CurrentTrainerUid",
                schema: "Pokemon",
                table: "Pokemon",
                column: "CurrentTrainerUid");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_OriginalTrainerId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "OriginalTrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_OriginalTrainerUid",
                schema: "Pokemon",
                table: "Pokemon",
                column: "OriginalTrainerUid");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_PokeBallId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "PokeBallId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_PokeBallUid",
                schema: "Pokemon",
                table: "Pokemon",
                column: "PokeBallUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Items_PokeBallId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "PokeBallId",
                principalSchema: "Pokemon",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Trainers_CurrentTrainerId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "CurrentTrainerId",
                principalSchema: "Pokemon",
                principalTable: "Trainers",
                principalColumn: "TrainerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Trainers_OriginalTrainerId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "OriginalTrainerId",
                principalSchema: "Pokemon",
                principalTable: "Trainers",
                principalColumn: "TrainerId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Items_PokeBallId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Trainers_CurrentTrainerId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Trainers_OriginalTrainerId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_CurrentTrainerId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_CurrentTrainerUid",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_OriginalTrainerId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_OriginalTrainerUid",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_PokeBallId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_PokeBallUid",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "CurrentTrainerId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "CurrentTrainerUid",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "MetAtLevel",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "MetDescription",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "MetLocation",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "MetOn",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "OriginalTrainerId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "OriginalTrainerUid",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "PokeBallId",
                schema: "Pokemon",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "PokeBallUid",
                schema: "Pokemon",
                table: "Pokemon");
        }
    }
}
