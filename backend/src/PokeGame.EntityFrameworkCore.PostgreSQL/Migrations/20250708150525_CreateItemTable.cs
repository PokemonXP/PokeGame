using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                schema: "Pokemon",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_Category",
                schema: "Pokemon",
                table: "Items",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedBy",
                schema: "Pokemon",
                table: "Items",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedOn",
                schema: "Pokemon",
                table: "Items",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Items_DisplayName",
                schema: "Pokemon",
                table: "Items",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Id",
                schema: "Pokemon",
                table: "Items",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Price",
                schema: "Pokemon",
                table: "Items",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Items_StreamId",
                schema: "Pokemon",
                table: "Items",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UniqueName",
                schema: "Pokemon",
                table: "Items",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UniqueNameNormalized",
                schema: "Pokemon",
                table: "Items",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UpdatedBy",
                schema: "Pokemon",
                table: "Items",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UpdatedOn",
                schema: "Pokemon",
                table: "Items",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Version",
                schema: "Pokemon",
                table: "Items",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items",
                schema: "Pokemon");
        }
    }
}
