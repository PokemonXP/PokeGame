using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreatePokemonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Varieties_VarietyId",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Varieties_Species_SpeciesId",
                schema: "Pokemon",
                table: "Varieties");

            migrationBuilder.CreateTable(
                name: "Pokemon",
                schema: "Pokemon",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesUid = table.Column<Guid>(type: "uuid", nullable: false),
                    VarietyId = table.Column<int>(type: "integer", nullable: false),
                    VarietyUid = table.Column<Guid>(type: "uuid", nullable: false),
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    FormUid = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Nickname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Gender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TeraType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Height = table.Column<byte>(type: "smallint", nullable: false),
                    Weight = table.Column<byte>(type: "smallint", nullable: false),
                    AbilitySlot = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Nature = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    GrowthRate = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    MaximumExperience = table.Column<int>(type: "integer", nullable: false),
                    ToNextLevel = table.Column<int>(type: "integer", nullable: false),
                    Statistics = table.Column<string>(type: "text", nullable: true),
                    Vitality = table.Column<int>(type: "integer", nullable: false),
                    Stamina = table.Column<int>(type: "integer", nullable: false),
                    Friendship = table.Column<byte>(type: "smallint", nullable: false),
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
                    table.PrimaryKey("PK_Pokemon", x => x.PokemonId);
                    table.ForeignKey(
                        name: "FK_Pokemon_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Pokemon",
                        principalTable: "Forms",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalSchema: "Pokemon",
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "Pokemon",
                        principalTable: "Varieties",
                        principalColumn: "VarietyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_CreatedBy",
                schema: "Pokemon",
                table: "Pokemon",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_CreatedOn",
                schema: "Pokemon",
                table: "Pokemon",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_FormId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_FormUid",
                schema: "Pokemon",
                table: "Pokemon",
                column: "FormUid");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Gender",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Id",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Level",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Nature",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Nature");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Nickname",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Nickname");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_SpeciesId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_SpeciesUid",
                schema: "Pokemon",
                table: "Pokemon",
                column: "SpeciesUid");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_StreamId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_TeraType",
                schema: "Pokemon",
                table: "Pokemon",
                column: "TeraType");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_UniqueName",
                schema: "Pokemon",
                table: "Pokemon",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_UniqueNameNormalized",
                schema: "Pokemon",
                table: "Pokemon",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_UpdatedBy",
                schema: "Pokemon",
                table: "Pokemon",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_UpdatedOn",
                schema: "Pokemon",
                table: "Pokemon",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_VarietyId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "VarietyId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_VarietyUid",
                schema: "Pokemon",
                table: "Pokemon",
                column: "VarietyUid");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Version",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Version");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Varieties_VarietyId",
                schema: "Pokemon",
                table: "Forms",
                column: "VarietyId",
                principalSchema: "Pokemon",
                principalTable: "Varieties",
                principalColumn: "VarietyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Varieties_Species_SpeciesId",
                schema: "Pokemon",
                table: "Varieties",
                column: "SpeciesId",
                principalSchema: "Pokemon",
                principalTable: "Species",
                principalColumn: "SpeciesId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Varieties_VarietyId",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Varieties_Species_SpeciesId",
                schema: "Pokemon",
                table: "Varieties");

            migrationBuilder.DropTable(
                name: "Pokemon",
                schema: "Pokemon");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Varieties_VarietyId",
                schema: "Pokemon",
                table: "Forms",
                column: "VarietyId",
                principalSchema: "Pokemon",
                principalTable: "Varieties",
                principalColumn: "VarietyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Varieties_Species_SpeciesId",
                schema: "Pokemon",
                table: "Varieties",
                column: "SpeciesId",
                principalSchema: "Pokemon",
                principalTable: "Species",
                principalColumn: "SpeciesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
