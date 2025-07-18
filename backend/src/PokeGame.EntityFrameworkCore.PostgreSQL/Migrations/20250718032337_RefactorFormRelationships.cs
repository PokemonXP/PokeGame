using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorFormRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RegionalNumbers",
                schema: "Pokemon",
                table: "RegionalNumbers");

            migrationBuilder.DropIndex(
                name: "IX_RegionalNumbers_SpeciesId_RegionId",
                schema: "Pokemon",
                table: "RegionalNumbers");

            migrationBuilder.DropIndex(
                name: "IX_Forms_ExperienceYield",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormAbilities",
                schema: "Pokemon",
                table: "FormAbilities");

            migrationBuilder.DropIndex(
                name: "IX_FormAbilities_FormId_AbilityId",
                schema: "Pokemon",
                table: "FormAbilities");

            migrationBuilder.DropColumn(
                name: "RegionalNumberId",
                schema: "Pokemon",
                table: "RegionalNumbers");

            migrationBuilder.DropColumn(
                name: "AttackBase",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "AttackYield",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "DefenseBase",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "DefenseYield",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "ExperienceYield",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "FormAbilityId",
                schema: "Pokemon",
                table: "FormAbilities");

            migrationBuilder.RenameColumn(
                name: "SpeedYield",
                schema: "Pokemon",
                table: "Forms",
                newName: "YieldSpeed");

            migrationBuilder.RenameColumn(
                name: "SpeedBase",
                schema: "Pokemon",
                table: "Forms",
                newName: "YieldSpecialDefense");

            migrationBuilder.RenameColumn(
                name: "SpecialDefenseYield",
                schema: "Pokemon",
                table: "Forms",
                newName: "YieldSpecialAttack");

            migrationBuilder.RenameColumn(
                name: "SpecialDefenseBase",
                schema: "Pokemon",
                table: "Forms",
                newName: "YieldHP");

            migrationBuilder.RenameColumn(
                name: "SpecialAttackYield",
                schema: "Pokemon",
                table: "Forms",
                newName: "YieldExperience");

            migrationBuilder.RenameColumn(
                name: "SpecialAttackBase",
                schema: "Pokemon",
                table: "Forms",
                newName: "YieldDefense");

            migrationBuilder.RenameColumn(
                name: "HPYield",
                schema: "Pokemon",
                table: "Forms",
                newName: "YieldAttack");

            migrationBuilder.RenameColumn(
                name: "HPBase",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpeciesId");

            migrationBuilder.RenameColumn(
                name: "DefaultSpriteShiny",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpriteShiny");

            migrationBuilder.RenameColumn(
                name: "DefaultSprite",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpriteDefault");

            migrationBuilder.RenameColumn(
                name: "AlternativeSpriteShiny",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpriteAlternativeShiny");

            migrationBuilder.RenameColumn(
                name: "AlternativeSprite",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpriteAlternative");

            migrationBuilder.AddColumn<byte>(
                name: "BaseAttack",
                schema: "Pokemon",
                table: "Forms",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "BaseDefense",
                schema: "Pokemon",
                table: "Forms",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "BaseHP",
                schema: "Pokemon",
                table: "Forms",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "BaseSpecialAttack",
                schema: "Pokemon",
                table: "Forms",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "BaseSpecialDefense",
                schema: "Pokemon",
                table: "Forms",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "BaseSpeed",
                schema: "Pokemon",
                table: "Forms",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<Guid>(
                name: "SpeciesUid",
                schema: "Pokemon",
                table: "Forms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegionalNumbers",
                schema: "Pokemon",
                table: "RegionalNumbers",
                columns: new[] { "SpeciesId", "RegionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormAbilities",
                schema: "Pokemon",
                table: "FormAbilities",
                columns: new[] { "FormId", "AbilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Forms_SpeciesId",
                schema: "Pokemon",
                table: "Forms",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_YieldExperience",
                schema: "Pokemon",
                table: "Forms",
                column: "YieldExperience");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Species_SpeciesId",
                schema: "Pokemon",
                table: "Forms",
                column: "SpeciesId",
                principalSchema: "Pokemon",
                principalTable: "Species",
                principalColumn: "SpeciesId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Species_SpeciesId",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegionalNumbers",
                schema: "Pokemon",
                table: "RegionalNumbers");

            migrationBuilder.DropIndex(
                name: "IX_Forms_SpeciesId",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Forms_YieldExperience",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormAbilities",
                schema: "Pokemon",
                table: "FormAbilities");

            migrationBuilder.DropColumn(
                name: "BaseAttack",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "BaseDefense",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "BaseHP",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "BaseSpecialAttack",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "BaseSpecialDefense",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "BaseSpeed",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "SpeciesUid",
                schema: "Pokemon",
                table: "Forms");

            migrationBuilder.RenameColumn(
                name: "YieldSpeed",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpeedYield");

            migrationBuilder.RenameColumn(
                name: "YieldSpecialDefense",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpeedBase");

            migrationBuilder.RenameColumn(
                name: "YieldSpecialAttack",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpecialDefenseYield");

            migrationBuilder.RenameColumn(
                name: "YieldHP",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpecialDefenseBase");

            migrationBuilder.RenameColumn(
                name: "YieldExperience",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpecialAttackYield");

            migrationBuilder.RenameColumn(
                name: "YieldDefense",
                schema: "Pokemon",
                table: "Forms",
                newName: "SpecialAttackBase");

            migrationBuilder.RenameColumn(
                name: "YieldAttack",
                schema: "Pokemon",
                table: "Forms",
                newName: "HPYield");

            migrationBuilder.RenameColumn(
                name: "SpriteShiny",
                schema: "Pokemon",
                table: "Forms",
                newName: "DefaultSpriteShiny");

            migrationBuilder.RenameColumn(
                name: "SpriteDefault",
                schema: "Pokemon",
                table: "Forms",
                newName: "DefaultSprite");

            migrationBuilder.RenameColumn(
                name: "SpriteAlternativeShiny",
                schema: "Pokemon",
                table: "Forms",
                newName: "AlternativeSpriteShiny");

            migrationBuilder.RenameColumn(
                name: "SpriteAlternative",
                schema: "Pokemon",
                table: "Forms",
                newName: "AlternativeSprite");

            migrationBuilder.RenameColumn(
                name: "SpeciesId",
                schema: "Pokemon",
                table: "Forms",
                newName: "HPBase");

            migrationBuilder.AddColumn<int>(
                name: "RegionalNumberId",
                schema: "Pokemon",
                table: "RegionalNumbers",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "AttackBase",
                schema: "Pokemon",
                table: "Forms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttackYield",
                schema: "Pokemon",
                table: "Forms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefenseBase",
                schema: "Pokemon",
                table: "Forms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefenseYield",
                schema: "Pokemon",
                table: "Forms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExperienceYield",
                schema: "Pokemon",
                table: "Forms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FormAbilityId",
                schema: "Pokemon",
                table: "FormAbilities",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegionalNumbers",
                schema: "Pokemon",
                table: "RegionalNumbers",
                column: "RegionalNumberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormAbilities",
                schema: "Pokemon",
                table: "FormAbilities",
                column: "FormAbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionalNumbers_SpeciesId_RegionId",
                schema: "Pokemon",
                table: "RegionalNumbers",
                columns: new[] { "SpeciesId", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_ExperienceYield",
                schema: "Pokemon",
                table: "Forms",
                column: "ExperienceYield");

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_FormId_AbilityId",
                schema: "Pokemon",
                table: "FormAbilities",
                columns: new[] { "FormId", "AbilityId" },
                unique: true);
        }
    }
}
