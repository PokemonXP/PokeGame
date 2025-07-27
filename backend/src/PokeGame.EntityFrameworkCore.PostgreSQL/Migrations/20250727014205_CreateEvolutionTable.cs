using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateEvolutionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VarietyMoves_VarietyUid",
                schema: "Pokemon",
                table: "VarietyMoves");

            migrationBuilder.DropIndex(
                name: "IX_RegionalNumbers_RegionUid",
                schema: "Pokemon",
                table: "RegionalNumbers");

            migrationBuilder.DropIndex(
                name: "IX_RegionalNumbers_SpeciesUid",
                schema: "Pokemon",
                table: "RegionalNumbers");

            migrationBuilder.DropIndex(
                name: "IX_PokemonMoves_PokemonUid",
                schema: "Pokemon",
                table: "PokemonMoves");

            migrationBuilder.DropIndex(
                name: "IX_FormAbilities_FormUid",
                schema: "Pokemon",
                table: "FormAbilities");

            migrationBuilder.CreateTable(
                name: "Evolutions",
                schema: "Pokemon",
                columns: table => new
                {
                    EvolutionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceId = table.Column<int>(type: "integer", nullable: false),
                    SourceUid = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetId = table.Column<int>(type: "integer", nullable: false),
                    TargetUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Trigger = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    ItemUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Friendship = table.Column<bool>(type: "boolean", nullable: false),
                    Gender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TimeOfDay = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HeldItemId = table.Column<int>(type: "integer", nullable: true),
                    HeldItemUid = table.Column<Guid>(type: "uuid", nullable: true),
                    KnownMoveId = table.Column<int>(type: "integer", nullable: true),
                    KnownMoveUid = table.Column<Guid>(type: "uuid", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evolutions", x => x.EvolutionId);
                    table.ForeignKey(
                        name: "FK_Evolutions_Forms_SourceId",
                        column: x => x.SourceId,
                        principalSchema: "Pokemon",
                        principalTable: "Forms",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evolutions_Forms_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "Pokemon",
                        principalTable: "Forms",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evolutions_Items_HeldItemId",
                        column: x => x.HeldItemId,
                        principalSchema: "Pokemon",
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evolutions_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Pokemon",
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evolutions_Moves_KnownMoveId",
                        column: x => x.KnownMoveId,
                        principalSchema: "Pokemon",
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_VarietyUid_MoveUid",
                schema: "Pokemon",
                table: "VarietyMoves",
                columns: new[] { "VarietyUid", "MoveUid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_RegionUid_Number",
                schema: "Pokemon",
                table: "RegionalNumbers",
                columns: new[] { "RegionUid", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_SpeciesUid_RegionUid",
                schema: "Pokemon",
                table: "RegionalNumbers",
                columns: new[] { "SpeciesUid", "RegionUid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_PokemonUid_MoveUid",
                schema: "Pokemon",
                table: "PokemonMoves",
                columns: new[] { "PokemonUid", "MoveUid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_FormUid_AbilityUid",
                schema: "Pokemon",
                table: "FormAbilities",
                columns: new[] { "FormUid", "AbilityUid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_FormUid_Slot",
                schema: "Pokemon",
                table: "FormAbilities",
                columns: new[] { "FormUid", "Slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_CreatedBy",
                schema: "Pokemon",
                table: "Evolutions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_CreatedOn",
                schema: "Pokemon",
                table: "Evolutions",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_HeldItemId",
                schema: "Pokemon",
                table: "Evolutions",
                column: "HeldItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_HeldItemUid",
                schema: "Pokemon",
                table: "Evolutions",
                column: "HeldItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_Id",
                schema: "Pokemon",
                table: "Evolutions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_ItemId",
                schema: "Pokemon",
                table: "Evolutions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_ItemUid",
                schema: "Pokemon",
                table: "Evolutions",
                column: "ItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_KnownMoveId",
                schema: "Pokemon",
                table: "Evolutions",
                column: "KnownMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_KnownMoveUid",
                schema: "Pokemon",
                table: "Evolutions",
                column: "KnownMoveUid");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_Level",
                schema: "Pokemon",
                table: "Evolutions",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_SourceId",
                schema: "Pokemon",
                table: "Evolutions",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_SourceUid",
                schema: "Pokemon",
                table: "Evolutions",
                column: "SourceUid");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_StreamId",
                schema: "Pokemon",
                table: "Evolutions",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_TargetId",
                schema: "Pokemon",
                table: "Evolutions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_TargetUid",
                schema: "Pokemon",
                table: "Evolutions",
                column: "TargetUid");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_UpdatedBy",
                schema: "Pokemon",
                table: "Evolutions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_UpdatedOn",
                schema: "Pokemon",
                table: "Evolutions",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Evolutions_Version",
                schema: "Pokemon",
                table: "Evolutions",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evolutions",
                schema: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_VarietyMoves_VarietyUid_MoveUid",
                schema: "Pokemon",
                table: "VarietyMoves");

            migrationBuilder.DropIndex(
                name: "IX_RegionalNumbers_RegionUid_Number",
                schema: "Pokemon",
                table: "RegionalNumbers");

            migrationBuilder.DropIndex(
                name: "IX_RegionalNumbers_SpeciesUid_RegionUid",
                schema: "Pokemon",
                table: "RegionalNumbers");

            migrationBuilder.DropIndex(
                name: "IX_PokemonMoves_PokemonUid_MoveUid",
                schema: "Pokemon",
                table: "PokemonMoves");

            migrationBuilder.DropIndex(
                name: "IX_FormAbilities_FormUid_AbilityUid",
                schema: "Pokemon",
                table: "FormAbilities");

            migrationBuilder.DropIndex(
                name: "IX_FormAbilities_FormUid_Slot",
                schema: "Pokemon",
                table: "FormAbilities");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_VarietyUid",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "VarietyUid");

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_RegionUid",
                schema: "Pokemon",
                table: "RegionalNumbers",
                column: "RegionUid");

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_SpeciesUid",
                schema: "Pokemon",
                table: "RegionalNumbers",
                column: "SpeciesUid");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_PokemonUid",
                schema: "Pokemon",
                table: "PokemonMoves",
                column: "PokemonUid");

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_FormUid",
                schema: "Pokemon",
                table: "FormAbilities",
                column: "FormUid");
        }
    }
}
