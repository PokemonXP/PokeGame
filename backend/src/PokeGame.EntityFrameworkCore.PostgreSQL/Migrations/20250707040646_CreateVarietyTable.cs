using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateVarietyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Varieties",
                schema: "Pokemon",
                columns: table => new
                {
                    VarietyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CanChangeForm = table.Column<bool>(type: "boolean", nullable: false),
                    GenderRatio = table.Column<int>(type: "integer", nullable: true),
                    Genus = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
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
                    table.PrimaryKey("PK_Varieties", x => x.VarietyId);
                    table.ForeignKey(
                        name: "FK_Varieties_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalSchema: "Pokemon",
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_CreatedBy",
                schema: "Pokemon",
                table: "Varieties",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_CreatedOn",
                schema: "Pokemon",
                table: "Varieties",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_DisplayName",
                schema: "Pokemon",
                table: "Varieties",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_Genus",
                schema: "Pokemon",
                table: "Varieties",
                column: "Genus");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_Id",
                schema: "Pokemon",
                table: "Varieties",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_SpeciesId_IsDefault",
                schema: "Pokemon",
                table: "Varieties",
                columns: new[] { "SpeciesId", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_SpeciesUid",
                schema: "Pokemon",
                table: "Varieties",
                column: "SpeciesUid");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_StreamId",
                schema: "Pokemon",
                table: "Varieties",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_UniqueName",
                schema: "Pokemon",
                table: "Varieties",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_UniqueNameNormalized",
                schema: "Pokemon",
                table: "Varieties",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_UpdatedBy",
                schema: "Pokemon",
                table: "Varieties",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_UpdatedOn",
                schema: "Pokemon",
                table: "Varieties",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_Version",
                schema: "Pokemon",
                table: "Varieties",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Varieties",
                schema: "Pokemon");
        }
    }
}
