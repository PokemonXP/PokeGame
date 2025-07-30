using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateBattleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Battles",
                schema: "Pokemon",
                columns: table => new
                {
                    BattleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_Battles", x => x.BattleId);
                });

            migrationBuilder.CreateTable(
                name: "BattlePokemon",
                schema: "Pokemon",
                columns: table => new
                {
                    BattleId = table.Column<int>(type: "integer", nullable: false),
                    PokemonId = table.Column<int>(type: "integer", nullable: false),
                    BattleUid = table.Column<Guid>(type: "uuid", nullable: false),
                    PokemonUid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattlePokemon", x => new { x.BattleId, x.PokemonId });
                    table.ForeignKey(
                        name: "FK_BattlePokemon_Battles_BattleId",
                        column: x => x.BattleId,
                        principalSchema: "Pokemon",
                        principalTable: "Battles",
                        principalColumn: "BattleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattlePokemon_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalSchema: "Pokemon",
                        principalTable: "Pokemon",
                        principalColumn: "PokemonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BattleTrainers",
                schema: "Pokemon",
                columns: table => new
                {
                    BattleId = table.Column<int>(type: "integer", nullable: false),
                    TrainerId = table.Column<int>(type: "integer", nullable: false),
                    BattleUid = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsOpponent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleTrainers", x => new { x.BattleId, x.TrainerId });
                    table.ForeignKey(
                        name: "FK_BattleTrainers_Battles_BattleId",
                        column: x => x.BattleId,
                        principalSchema: "Pokemon",
                        principalTable: "Battles",
                        principalColumn: "BattleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattleTrainers_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "Pokemon",
                        principalTable: "Trainers",
                        principalColumn: "TrainerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BattlePokemon_BattleUid_PokemonUid",
                schema: "Pokemon",
                table: "BattlePokemon",
                columns: new[] { "BattleUid", "PokemonUid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattlePokemon_PokemonId",
                schema: "Pokemon",
                table: "BattlePokemon",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_BattlePokemon_PokemonUid",
                schema: "Pokemon",
                table: "BattlePokemon",
                column: "PokemonUid");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CreatedBy",
                schema: "Pokemon",
                table: "Battles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CreatedOn",
                schema: "Pokemon",
                table: "Battles",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Id",
                schema: "Pokemon",
                table: "Battles",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Kind",
                schema: "Pokemon",
                table: "Battles",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Location",
                schema: "Pokemon",
                table: "Battles",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Name",
                schema: "Pokemon",
                table: "Battles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Status",
                schema: "Pokemon",
                table: "Battles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_StreamId",
                schema: "Pokemon",
                table: "Battles",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battles_UpdatedBy",
                schema: "Pokemon",
                table: "Battles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_UpdatedOn",
                schema: "Pokemon",
                table: "Battles",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Version",
                schema: "Pokemon",
                table: "Battles",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_BattleTrainers_BattleUid_TrainerUid",
                schema: "Pokemon",
                table: "BattleTrainers",
                columns: new[] { "BattleUid", "TrainerUid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattleTrainers_IsOpponent",
                schema: "Pokemon",
                table: "BattleTrainers",
                column: "IsOpponent");

            migrationBuilder.CreateIndex(
                name: "IX_BattleTrainers_TrainerId",
                schema: "Pokemon",
                table: "BattleTrainers",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleTrainers_TrainerUid",
                schema: "Pokemon",
                table: "BattleTrainers",
                column: "TrainerUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattlePokemon",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "BattleTrainers",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Battles",
                schema: "Pokemon");
        }
    }
}
