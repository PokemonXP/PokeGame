using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateRegionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Pokemon");

            migrationBuilder.CreateTable(
                name: "Regions",
                schema: "Pokemon",
                columns: table => new
                {
                    RegionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Regions", x => x.RegionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CreatedBy",
                schema: "Pokemon",
                table: "Regions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CreatedOn",
                schema: "Pokemon",
                table: "Regions",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_DisplayName",
                schema: "Pokemon",
                table: "Regions",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Id",
                schema: "Pokemon",
                table: "Regions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_StreamId",
                schema: "Pokemon",
                table: "Regions",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_UniqueName",
                schema: "Pokemon",
                table: "Regions",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_UniqueNameNormalized",
                schema: "Pokemon",
                table: "Regions",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_UpdatedBy",
                schema: "Pokemon",
                table: "Regions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_UpdatedOn",
                schema: "Pokemon",
                table: "Regions",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Version",
                schema: "Pokemon",
                table: "Regions",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Regions",
                schema: "Pokemon");
        }
    }
}
