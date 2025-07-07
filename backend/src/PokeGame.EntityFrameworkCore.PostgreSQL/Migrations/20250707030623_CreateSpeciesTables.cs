using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateSpeciesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Species",
                schema: "Pokemon",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BaseFriendship = table.Column<int>(type: "integer", nullable: false),
                    CatchRate = table.Column<int>(type: "integer", nullable: false),
                    GrowthRate = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_Species", x => x.SpeciesId);
                });

            migrationBuilder.CreateTable(
                name: "RegionalNumbers",
                schema: "Pokemon",
                columns: table => new
                {
                    RegionalNumberId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesUid = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<int>(type: "integer", nullable: false),
                    RegionUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalNumbers", x => x.RegionalNumberId);
                    table.ForeignKey(
                        name: "FK_RegionalNumbers_Regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "Pokemon",
                        principalTable: "Regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegionalNumbers_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalSchema: "Pokemon",
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_RegionId_Number",
                schema: "Pokemon",
                table: "RegionalNumbers",
                columns: new[] { "RegionId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_RegionUid",
                schema: "Pokemon",
                table: "RegionalNumbers",
                column: "RegionUid");

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_SpeciesId_RegionId",
                schema: "Pokemon",
                table: "RegionalNumbers",
                columns: new[] { "SpeciesId", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_SpeciesUid",
                schema: "Pokemon",
                table: "RegionalNumbers",
                column: "SpeciesUid");

            migrationBuilder.CreateIndex(
                name: "IX_Species_BaseFriendship",
                schema: "Pokemon",
                table: "Species",
                column: "BaseFriendship");

            migrationBuilder.CreateIndex(
                name: "IX_Species_CatchRate",
                schema: "Pokemon",
                table: "Species",
                column: "CatchRate");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Category",
                schema: "Pokemon",
                table: "Species",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Species_CreatedBy",
                schema: "Pokemon",
                table: "Species",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Species_CreatedOn",
                schema: "Pokemon",
                table: "Species",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Species_DisplayName",
                schema: "Pokemon",
                table: "Species",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Species_GrowthRate",
                schema: "Pokemon",
                table: "Species",
                column: "GrowthRate");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Id",
                schema: "Pokemon",
                table: "Species",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_Number",
                schema: "Pokemon",
                table: "Species",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_StreamId",
                schema: "Pokemon",
                table: "Species",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_UniqueName",
                schema: "Pokemon",
                table: "Species",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Species_UniqueNameNormalized",
                schema: "Pokemon",
                table: "Species",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_UpdatedBy",
                schema: "Pokemon",
                table: "Species",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Species_UpdatedOn",
                schema: "Pokemon",
                table: "Species",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Version",
                schema: "Pokemon",
                table: "Species",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionalNumbers",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Species",
                schema: "Pokemon");
        }
    }
}
