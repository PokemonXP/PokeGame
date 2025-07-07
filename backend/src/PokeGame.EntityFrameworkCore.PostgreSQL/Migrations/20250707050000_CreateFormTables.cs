using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateFormTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forms",
                schema: "Pokemon",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VarietyId = table.Column<int>(type: "integer", nullable: false),
                    VarietyUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsBattleOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsMega = table.Column<bool>(type: "boolean", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    PrimaryType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SecondaryType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HPBase = table.Column<int>(type: "integer", nullable: false),
                    AttackBase = table.Column<int>(type: "integer", nullable: false),
                    DefenseBase = table.Column<int>(type: "integer", nullable: false),
                    SpecialAttackBase = table.Column<int>(type: "integer", nullable: false),
                    SpecialDefenseBase = table.Column<int>(type: "integer", nullable: false),
                    SpeedBase = table.Column<int>(type: "integer", nullable: false),
                    ExperienceYield = table.Column<int>(type: "integer", nullable: false),
                    HPYield = table.Column<int>(type: "integer", nullable: false),
                    AttackYield = table.Column<int>(type: "integer", nullable: false),
                    DefenseYield = table.Column<int>(type: "integer", nullable: false),
                    SpecialAttackYield = table.Column<int>(type: "integer", nullable: false),
                    SpecialDefenseYield = table.Column<int>(type: "integer", nullable: false),
                    SpeedYield = table.Column<int>(type: "integer", nullable: false),
                    DefaultSprite = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    DefaultSpriteShiny = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    AlternativeSprite = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    AlternativeSpriteShiny = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
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
                    table.PrimaryKey("PK_Forms", x => x.FormId);
                    table.ForeignKey(
                        name: "FK_Forms_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "Pokemon",
                        principalTable: "Varieties",
                        principalColumn: "VarietyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormAbilities",
                schema: "Pokemon",
                columns: table => new
                {
                    FormAbilityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    FormUid = table.Column<Guid>(type: "uuid", nullable: false),
                    AbilityId = table.Column<int>(type: "integer", nullable: false),
                    AbilityUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Slot = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormAbilities", x => x.FormAbilityId);
                    table.ForeignKey(
                        name: "FK_FormAbilities_Abilities_AbilityId",
                        column: x => x.AbilityId,
                        principalSchema: "Pokemon",
                        principalTable: "Abilities",
                        principalColumn: "AbilityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormAbilities_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Pokemon",
                        principalTable: "Forms",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_AbilityId",
                schema: "Pokemon",
                table: "FormAbilities",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_AbilityUid",
                schema: "Pokemon",
                table: "FormAbilities",
                column: "AbilityUid");

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_FormId_AbilityId",
                schema: "Pokemon",
                table: "FormAbilities",
                columns: new[] { "FormId", "AbilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_FormId_Slot",
                schema: "Pokemon",
                table: "FormAbilities",
                columns: new[] { "FormId", "Slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_FormUid",
                schema: "Pokemon",
                table: "FormAbilities",
                column: "FormUid");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_CreatedBy",
                schema: "Pokemon",
                table: "Forms",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_CreatedOn",
                schema: "Pokemon",
                table: "Forms",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_DisplayName",
                schema: "Pokemon",
                table: "Forms",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Id",
                schema: "Pokemon",
                table: "Forms",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_StreamId",
                schema: "Pokemon",
                table: "Forms",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UniqueName",
                schema: "Pokemon",
                table: "Forms",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UniqueNameNormalized",
                schema: "Pokemon",
                table: "Forms",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UpdatedBy",
                schema: "Pokemon",
                table: "Forms",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UpdatedOn",
                schema: "Pokemon",
                table: "Forms",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_VarietyId_IsDefault",
                schema: "Pokemon",
                table: "Forms",
                columns: new[] { "VarietyId", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_Forms_VarietyUid",
                schema: "Pokemon",
                table: "Forms",
                column: "VarietyUid");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Version",
                schema: "Pokemon",
                table: "Forms",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormAbilities",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Forms",
                schema: "Pokemon");
        }
    }
}
