using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class PokeGame_1_0_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Pokemon");

            migrationBuilder.CreateTable(
                name: "Abilities",
                schema: "Pokemon",
                columns: table => new
                {
                    AbilityId = table.Column<int>(type: "integer", nullable: false)
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
                    table.PrimaryKey("PK_Abilities", x => x.AbilityId);
                });

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
                    Resolution = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ChampionCount = table.Column<int>(type: "integer", nullable: false),
                    OpponentCount = table.Column<int>(type: "integer", nullable: false),
                    StartedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StartedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CancelledOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                name: "Moves",
                schema: "Pokemon",
                columns: table => new
                {
                    MoveId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Accuracy = table.Column<byte>(type: "smallint", nullable: true),
                    Power = table.Column<byte>(type: "smallint", nullable: true),
                    PowerPoints = table.Column<byte>(type: "smallint", nullable: false),
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
                    table.PrimaryKey("PK_Moves", x => x.MoveId);
                });

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

            migrationBuilder.CreateTable(
                name: "Species",
                schema: "Pokemon",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    BaseFriendship = table.Column<byte>(type: "smallint", nullable: false),
                    CatchRate = table.Column<byte>(type: "smallint", nullable: false),
                    GrowthRate = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EggCycles = table.Column<byte>(type: "smallint", nullable: false),
                    PrimaryEggGroup = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SecondaryEggGroup = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                name: "Trainers",
                schema: "Pokemon",
                columns: table => new
                {
                    TrainerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    License = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    LicenseNormalized = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Money = table.Column<int>(type: "integer", nullable: false),
                    PartySize = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserUid = table.Column<Guid>(type: "uuid", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "Items",
                schema: "Pokemon",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Sprite = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    MoveId = table.Column<int>(type: "integer", nullable: true),
                    MoveUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_Items_Moves_MoveId",
                        column: x => x.MoveId,
                        principalSchema: "Pokemon",
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegionalNumbers",
                schema: "Pokemon",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    RegionId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesUid = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalNumbers", x => new { x.SpeciesId, x.RegionId });
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
                    Genus = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GenderRatio = table.Column<int>(type: "integer", nullable: true),
                    CanChangeForm = table.Column<bool>(type: "boolean", nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "Inventory",
                schema: "Pokemon",
                columns: table => new
                {
                    TrainerId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    TrainerUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => new { x.TrainerId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_Inventory_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Pokemon",
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "Pokemon",
                        principalTable: "Trainers",
                        principalColumn: "TrainerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                schema: "Pokemon",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    SpeciesUid = table.Column<Guid>(type: "uuid", nullable: false),
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
                    BaseHP = table.Column<byte>(type: "smallint", nullable: false),
                    BaseAttack = table.Column<byte>(type: "smallint", nullable: false),
                    BaseDefense = table.Column<byte>(type: "smallint", nullable: false),
                    BaseSpecialAttack = table.Column<byte>(type: "smallint", nullable: false),
                    BaseSpecialDefense = table.Column<byte>(type: "smallint", nullable: false),
                    BaseSpeed = table.Column<byte>(type: "smallint", nullable: false),
                    YieldExperience = table.Column<int>(type: "integer", nullable: false),
                    YieldHP = table.Column<int>(type: "integer", nullable: false),
                    YieldAttack = table.Column<int>(type: "integer", nullable: false),
                    YieldDefense = table.Column<int>(type: "integer", nullable: false),
                    YieldSpecialAttack = table.Column<int>(type: "integer", nullable: false),
                    YieldSpecialDefense = table.Column<int>(type: "integer", nullable: false),
                    YieldSpeed = table.Column<int>(type: "integer", nullable: false),
                    SpriteDefault = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    SpriteShiny = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    SpriteAlternative = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    SpriteAlternativeShiny = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
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
                        name: "FK_Forms_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalSchema: "Pokemon",
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Forms_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "Pokemon",
                        principalTable: "Varieties",
                        principalColumn: "VarietyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VarietyMoves",
                schema: "Pokemon",
                columns: table => new
                {
                    VarietyId = table.Column<int>(type: "integer", nullable: false),
                    MoveId = table.Column<int>(type: "integer", nullable: false),
                    VarietyUid = table.Column<Guid>(type: "uuid", nullable: false),
                    MoveUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarietyMoves", x => new { x.VarietyId, x.MoveId });
                    table.ForeignKey(
                        name: "FK_VarietyMoves_Moves_MoveId",
                        column: x => x.MoveId,
                        principalSchema: "Pokemon",
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VarietyMoves_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "Pokemon",
                        principalTable: "Varieties",
                        principalColumn: "VarietyId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "FormAbilities",
                schema: "Pokemon",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    AbilityId = table.Column<int>(type: "integer", nullable: false),
                    FormUid = table.Column<Guid>(type: "uuid", nullable: false),
                    AbilityUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Slot = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormAbilities", x => new { x.FormId, x.AbilityId });
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
                    IsShiny = table.Column<bool>(type: "boolean", nullable: false),
                    TeraType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Height = table.Column<byte>(type: "smallint", nullable: false),
                    Weight = table.Column<byte>(type: "smallint", nullable: false),
                    AbilitySlot = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Nature = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    EggCycles = table.Column<byte>(type: "smallint", nullable: false),
                    IsEgg = table.Column<bool>(type: "boolean", nullable: false),
                    GrowthRate = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    MaximumExperience = table.Column<int>(type: "integer", nullable: false),
                    ToNextLevel = table.Column<int>(type: "integer", nullable: false),
                    Statistics = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Vitality = table.Column<int>(type: "integer", nullable: false),
                    Stamina = table.Column<int>(type: "integer", nullable: false),
                    StatusCondition = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Friendship = table.Column<byte>(type: "smallint", nullable: false),
                    Characteristic = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    HeldItemId = table.Column<int>(type: "integer", nullable: true),
                    HeldItemUid = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginalTrainerId = table.Column<int>(type: "integer", nullable: true),
                    OriginalTrainerUid = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrentTrainerId = table.Column<int>(type: "integer", nullable: true),
                    CurrentTrainerUid = table.Column<Guid>(type: "uuid", nullable: true),
                    PokeBallId = table.Column<int>(type: "integer", nullable: true),
                    PokeBallUid = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnershipKind = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MetAtLevel = table.Column<int>(type: "integer", nullable: true),
                    MetLocation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MetOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MetDescription = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: true),
                    Box = table.Column<int>(type: "integer", nullable: true),
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
                        name: "FK_Pokemon_Items_HeldItemId",
                        column: x => x.HeldItemId,
                        principalSchema: "Pokemon",
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Items_PokeBallId",
                        column: x => x.PokeBallId,
                        principalSchema: "Pokemon",
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalSchema: "Pokemon",
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Trainers_CurrentTrainerId",
                        column: x => x.CurrentTrainerId,
                        principalSchema: "Pokemon",
                        principalTable: "Trainers",
                        principalColumn: "TrainerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Trainers_OriginalTrainerId",
                        column: x => x.OriginalTrainerId,
                        principalSchema: "Pokemon",
                        principalTable: "Trainers",
                        principalColumn: "TrainerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemon_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "Pokemon",
                        principalTable: "Varieties",
                        principalColumn: "VarietyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BattlePokemon",
                schema: "Pokemon",
                columns: table => new
                {
                    BattleId = table.Column<int>(type: "integer", nullable: false),
                    PokemonId = table.Column<int>(type: "integer", nullable: false),
                    BattleUid = table.Column<Guid>(type: "uuid", nullable: false),
                    PokemonUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Attack = table.Column<int>(type: "integer", nullable: false),
                    Defense = table.Column<int>(type: "integer", nullable: false),
                    SpecialAttack = table.Column<int>(type: "integer", nullable: false),
                    SpecialDefense = table.Column<int>(type: "integer", nullable: false),
                    Speed = table.Column<int>(type: "integer", nullable: false),
                    Accuracy = table.Column<int>(type: "integer", nullable: false),
                    Evasion = table.Column<int>(type: "integer", nullable: false),
                    Critical = table.Column<int>(type: "integer", nullable: false)
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
                name: "PokemonMoves",
                schema: "Pokemon",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "integer", nullable: false),
                    MoveId = table.Column<int>(type: "integer", nullable: false),
                    PokemonUid = table.Column<Guid>(type: "uuid", nullable: false),
                    MoveUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: true),
                    CurrentPowerPoints = table.Column<byte>(type: "smallint", nullable: false),
                    MaximumPowerPoints = table.Column<byte>(type: "smallint", nullable: false),
                    ReferencePowerPoints = table.Column<byte>(type: "smallint", nullable: false),
                    IsMastered = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Method = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    ItemUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonMoves", x => new { x.PokemonId, x.MoveId });
                    table.ForeignKey(
                        name: "FK_PokemonMoves_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Pokemon",
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PokemonMoves_Moves_MoveId",
                        column: x => x.MoveId,
                        principalSchema: "Pokemon",
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonMoves_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalSchema: "Pokemon",
                        principalTable: "Pokemon",
                        principalColumn: "PokemonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_CreatedBy",
                schema: "Pokemon",
                table: "Abilities",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_CreatedOn",
                schema: "Pokemon",
                table: "Abilities",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_DisplayName",
                schema: "Pokemon",
                table: "Abilities",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_Id",
                schema: "Pokemon",
                table: "Abilities",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_StreamId",
                schema: "Pokemon",
                table: "Abilities",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_UniqueName",
                schema: "Pokemon",
                table: "Abilities",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_UniqueNameNormalized",
                schema: "Pokemon",
                table: "Abilities",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_UpdatedBy",
                schema: "Pokemon",
                table: "Abilities",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_UpdatedOn",
                schema: "Pokemon",
                table: "Abilities",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_Version",
                schema: "Pokemon",
                table: "Abilities",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_BattlePokemon_BattleUid_PokemonUid",
                schema: "Pokemon",
                table: "BattlePokemon",
                columns: new[] { "BattleUid", "PokemonUid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattlePokemon_IsActive",
                schema: "Pokemon",
                table: "BattlePokemon",
                column: "IsActive");

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
                name: "IX_Battles_CancelledBy",
                schema: "Pokemon",
                table: "Battles",
                column: "CancelledBy");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CancelledOn",
                schema: "Pokemon",
                table: "Battles",
                column: "CancelledOn");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_ChampionCount",
                schema: "Pokemon",
                table: "Battles",
                column: "ChampionCount");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CompletedBy",
                schema: "Pokemon",
                table: "Battles",
                column: "CompletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_CompletedOn",
                schema: "Pokemon",
                table: "Battles",
                column: "CompletedOn");

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
                name: "IX_Battles_OpponentCount",
                schema: "Pokemon",
                table: "Battles",
                column: "OpponentCount");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Resolution",
                schema: "Pokemon",
                table: "Battles",
                column: "Resolution");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_StartedBy",
                schema: "Pokemon",
                table: "Battles",
                column: "StartedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_StartedOn",
                schema: "Pokemon",
                table: "Battles",
                column: "StartedOn");

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
                name: "IX_FormAbilities_FormId_Slot",
                schema: "Pokemon",
                table: "FormAbilities",
                columns: new[] { "FormId", "Slot" },
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
                name: "IX_Forms_Height",
                schema: "Pokemon",
                table: "Forms",
                column: "Height");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Id",
                schema: "Pokemon",
                table: "Forms",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_SpeciesId",
                schema: "Pokemon",
                table: "Forms",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_SpeciesUid",
                schema: "Pokemon",
                table: "Forms",
                column: "SpeciesUid");

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

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Weight",
                schema: "Pokemon",
                table: "Forms",
                column: "Weight");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_YieldExperience",
                schema: "Pokemon",
                table: "Forms",
                column: "YieldExperience");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ItemId",
                schema: "Pokemon",
                table: "Inventory",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ItemUid",
                schema: "Pokemon",
                table: "Inventory",
                column: "ItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Quantity",
                schema: "Pokemon",
                table: "Inventory",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_TrainerUid",
                schema: "Pokemon",
                table: "Inventory",
                column: "TrainerUid");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_TrainerUid_ItemUid",
                schema: "Pokemon",
                table: "Inventory",
                columns: new[] { "TrainerUid", "ItemUid" },
                unique: true);

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
                name: "IX_Items_MoveId",
                schema: "Pokemon",
                table: "Items",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_MoveUid",
                schema: "Pokemon",
                table: "Items",
                column: "MoveUid");

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

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Accuracy",
                schema: "Pokemon",
                table: "Moves",
                column: "Accuracy");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Category",
                schema: "Pokemon",
                table: "Moves",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_CreatedBy",
                schema: "Pokemon",
                table: "Moves",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_CreatedOn",
                schema: "Pokemon",
                table: "Moves",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_DisplayName",
                schema: "Pokemon",
                table: "Moves",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Id",
                schema: "Pokemon",
                table: "Moves",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Power",
                schema: "Pokemon",
                table: "Moves",
                column: "Power");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_PowerPoints",
                schema: "Pokemon",
                table: "Moves",
                column: "PowerPoints");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_StreamId",
                schema: "Pokemon",
                table: "Moves",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Type",
                schema: "Pokemon",
                table: "Moves",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_UniqueName",
                schema: "Pokemon",
                table: "Moves",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_UniqueNameNormalized",
                schema: "Pokemon",
                table: "Moves",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_UpdatedBy",
                schema: "Pokemon",
                table: "Moves",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_UpdatedOn",
                schema: "Pokemon",
                table: "Moves",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Version",
                schema: "Pokemon",
                table: "Moves",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Box",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Box");

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
                name: "IX_Pokemon_EggCycles",
                schema: "Pokemon",
                table: "Pokemon",
                column: "EggCycles");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Experience",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Experience");

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
                name: "IX_Pokemon_HeldItemId",
                schema: "Pokemon",
                table: "Pokemon",
                column: "HeldItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_HeldItemUid",
                schema: "Pokemon",
                table: "Pokemon",
                column: "HeldItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Id",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_IsEgg",
                schema: "Pokemon",
                table: "Pokemon",
                column: "IsEgg");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Level",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Nickname",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Nickname");

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

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Position",
                schema: "Pokemon",
                table: "Pokemon",
                column: "Position");

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

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_ItemId",
                schema: "Pokemon",
                table: "PokemonMoves",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_ItemUid",
                schema: "Pokemon",
                table: "PokemonMoves",
                column: "ItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_MoveId",
                schema: "Pokemon",
                table: "PokemonMoves",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_MoveUid",
                schema: "Pokemon",
                table: "PokemonMoves",
                column: "MoveUid");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_PokemonUid_MoveUid",
                schema: "Pokemon",
                table: "PokemonMoves",
                columns: new[] { "PokemonUid", "MoveUid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_RegionId_Number",
                schema: "Pokemon",
                table: "RegionalNumbers",
                columns: new[] { "RegionId", "Number" },
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
                name: "IX_Species_EggCycles",
                schema: "Pokemon",
                table: "Species",
                column: "EggCycles");

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
                name: "IX_Species_PrimaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                column: "PrimaryEggGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Species_SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                column: "SecondaryEggGroup");

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
                name: "IX_Trainers_PartySize",
                schema: "Pokemon",
                table: "Trainers",
                column: "PartySize");

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
                name: "IX_Trainers_UserUid",
                schema: "Pokemon",
                table: "Trainers",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Version",
                schema: "Pokemon",
                table: "Trainers",
                column: "Version");

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

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_Level",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_MoveId",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_MoveUid",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "MoveUid");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_VarietyUid_MoveUid",
                schema: "Pokemon",
                table: "VarietyMoves",
                columns: new[] { "VarietyUid", "MoveUid" },
                unique: true);
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
                name: "Evolutions",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "FormAbilities",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Inventory",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "PokemonMoves",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "RegionalNumbers",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "VarietyMoves",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Battles",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Abilities",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Pokemon",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Regions",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Forms",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Items",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Trainers",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Varieties",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Moves",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Species",
                schema: "Pokemon");
        }
    }
}
