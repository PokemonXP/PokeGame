using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trainers",
                schema: "Pokemon",
                columns: table => new
                {
                    TrainerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    License = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    LicenseNormalized = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Money = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Sprite = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainers", x => x.TrainerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_CreatedBy",
                schema: "Pokemon",
                table: "Trainers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_CreatedOn",
                schema: "Pokemon",
                table: "Trainers",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_DisplayName",
                schema: "Pokemon",
                table: "Trainers",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Gender",
                schema: "Pokemon",
                table: "Trainers",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Id",
                schema: "Pokemon",
                table: "Trainers",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_License",
                schema: "Pokemon",
                table: "Trainers",
                column: "License");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_LicenseNormalized",
                schema: "Pokemon",
                table: "Trainers",
                column: "LicenseNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Money",
                schema: "Pokemon",
                table: "Trainers",
                column: "Money");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_StreamId",
                schema: "Pokemon",
                table: "Trainers",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_UniqueName",
                schema: "Pokemon",
                table: "Trainers",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_UniqueNameNormalized",
                schema: "Pokemon",
                table: "Trainers",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_UpdatedBy",
                schema: "Pokemon",
                table: "Trainers",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_UpdatedOn",
                schema: "Pokemon",
                table: "Trainers",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_UserId",
                schema: "Pokemon",
                table: "Trainers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Version",
                schema: "Pokemon",
                table: "Trainers",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trainers",
                schema: "Pokemon");
        }
    }
}
