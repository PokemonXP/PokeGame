﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PokeGame.EntityFrameworkCore;

#nullable disable

namespace PokeGame.EntityFrameworkCore.PostgreSQL.Migrations
{
    [DbContext(typeof(PokemonContext))]
    [Migration("20250707050000_CreateFormTables")]
    partial class CreateFormTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.AbilityEntity", b =>
                {
                    b.Property<int>("AbilityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AbilityId"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("AbilityId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UniqueNameNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("Abilities", "Pokemon");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.FormAbilityEntity", b =>
                {
                    b.Property<int>("FormAbilityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("FormAbilityId"));

                    b.Property<int>("AbilityId")
                        .HasColumnType("integer");

                    b.Property<Guid>("AbilityUid")
                        .HasColumnType("uuid");

                    b.Property<int>("FormId")
                        .HasColumnType("integer");

                    b.Property<Guid>("FormUid")
                        .HasColumnType("uuid");

                    b.Property<string>("Slot")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("FormAbilityId");

                    b.HasIndex("AbilityId");

                    b.HasIndex("AbilityUid");

                    b.HasIndex("FormUid");

                    b.HasIndex("FormId", "AbilityId")
                        .IsUnique();

                    b.HasIndex("FormId", "Slot")
                        .IsUnique();

                    b.ToTable("FormAbilities", "Pokemon");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.FormEntity", b =>
                {
                    b.Property<int>("FormId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("FormId"));

                    b.Property<string>("AlternativeSprite")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<string>("AlternativeSpriteShiny")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<int>("AttackBase")
                        .HasColumnType("integer");

                    b.Property<int>("AttackYield")
                        .HasColumnType("integer");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DefaultSprite")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<string>("DefaultSpriteShiny")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<int>("DefenseBase")
                        .HasColumnType("integer");

                    b.Property<int>("DefenseYield")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("ExperienceYield")
                        .HasColumnType("integer");

                    b.Property<int>("HPBase")
                        .HasColumnType("integer");

                    b.Property<int>("HPYield")
                        .HasColumnType("integer");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsBattleOnly")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsMega")
                        .HasColumnType("boolean");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<string>("PrimaryType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("SecondaryType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("SpecialAttackBase")
                        .HasColumnType("integer");

                    b.Property<int>("SpecialAttackYield")
                        .HasColumnType("integer");

                    b.Property<int>("SpecialDefenseBase")
                        .HasColumnType("integer");

                    b.Property<int>("SpecialDefenseYield")
                        .HasColumnType("integer");

                    b.Property<int>("SpeedBase")
                        .HasColumnType("integer");

                    b.Property<int>("SpeedYield")
                        .HasColumnType("integer");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<int>("VarietyId")
                        .HasColumnType("integer");

                    b.Property<Guid>("VarietyUid")
                        .HasColumnType("uuid");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<int>("Weight")
                        .HasColumnType("integer");

                    b.HasKey("FormId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UniqueNameNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("VarietyUid");

                    b.HasIndex("Version");

                    b.HasIndex("VarietyId", "IsDefault");

                    b.ToTable("Forms", "Pokemon");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.MoveEntity", b =>
                {
                    b.Property<int>("MoveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MoveId"));

                    b.Property<int>("Accuracy")
                        .HasColumnType("integer");

                    b.Property<int>("AccuracyChange")
                        .HasColumnType("integer");

                    b.Property<int>("AttackChange")
                        .HasColumnType("integer");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CriticalChange")
                        .HasColumnType("integer");

                    b.Property<int>("DefenseChange")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("EvasionChange")
                        .HasColumnType("integer");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<int>("Power")
                        .HasColumnType("integer");

                    b.Property<int>("PowerPoints")
                        .HasColumnType("integer");

                    b.Property<int>("SpecialAttackChange")
                        .HasColumnType("integer");

                    b.Property<int>("SpecialDefenseChange")
                        .HasColumnType("integer");

                    b.Property<int>("SpeedChange")
                        .HasColumnType("integer");

                    b.Property<int>("StatusChance")
                        .HasColumnType("integer");

                    b.Property<string>("StatusCondition")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<string>("VolatileConditions")
                        .HasColumnType("text");

                    b.HasKey("MoveId");

                    b.HasIndex("Accuracy");

                    b.HasIndex("Category");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Power");

                    b.HasIndex("PowerPoints");

                    b.HasIndex("StatusCondition");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("Type");

                    b.HasIndex("UniqueName");

                    b.HasIndex("UniqueNameNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("Moves", "Pokemon");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.RegionEntity", b =>
                {
                    b.Property<int>("RegionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RegionId"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("RegionId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UniqueNameNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("Regions", "Pokemon");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.RegionalNumberEntity", b =>
                {
                    b.Property<int>("RegionalNumberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RegionalNumberId"));

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("RegionId")
                        .HasColumnType("integer");

                    b.Property<Guid>("RegionUid")
                        .HasColumnType("uuid");

                    b.Property<int>("SpeciesId")
                        .HasColumnType("integer");

                    b.Property<Guid>("SpeciesUid")
                        .HasColumnType("uuid");

                    b.HasKey("RegionalNumberId");

                    b.HasIndex("RegionUid");

                    b.HasIndex("SpeciesUid");

                    b.HasIndex("RegionId", "Number")
                        .IsUnique();

                    b.HasIndex("SpeciesId", "RegionId")
                        .IsUnique();

                    b.ToTable("RegionalNumbers", "Pokemon");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.SpeciesEntity", b =>
                {
                    b.Property<int>("SpeciesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SpeciesId"));

                    b.Property<int>("BaseFriendship")
                        .HasColumnType("integer");

                    b.Property<int>("CatchRate")
                        .HasColumnType("integer");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("GrowthRate")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("SpeciesId");

                    b.HasIndex("BaseFriendship");

                    b.HasIndex("CatchRate");

                    b.HasIndex("Category");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("GrowthRate");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Number")
                        .IsUnique();

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UniqueNameNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("Species", "Pokemon");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.VarietyEntity", b =>
                {
                    b.Property<int>("VarietyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("VarietyId"));

                    b.Property<bool>("CanChangeForm")
                        .HasColumnType("boolean");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int?>("GenderRatio")
                        .HasColumnType("integer");

                    b.Property<string>("Genus")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<int>("SpeciesId")
                        .HasColumnType("integer");

                    b.Property<Guid>("SpeciesUid")
                        .HasColumnType("uuid");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("VarietyId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("Genus");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("SpeciesUid");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UniqueNameNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("SpeciesId", "IsDefault");

                    b.ToTable("Varieties", "Pokemon");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.FormAbilityEntity", b =>
                {
                    b.HasOne("PokeGame.EntityFrameworkCore.Entities.AbilityEntity", "Ability")
                        .WithMany("Forms")
                        .HasForeignKey("AbilityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PokeGame.EntityFrameworkCore.Entities.FormEntity", "Form")
                        .WithMany("Abilities")
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ability");

                    b.Navigation("Form");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.FormEntity", b =>
                {
                    b.HasOne("PokeGame.EntityFrameworkCore.Entities.VarietyEntity", "Variety")
                        .WithMany("Forms")
                        .HasForeignKey("VarietyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Variety");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.RegionalNumberEntity", b =>
                {
                    b.HasOne("PokeGame.EntityFrameworkCore.Entities.RegionEntity", "Region")
                        .WithMany("RegionalNumbers")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PokeGame.EntityFrameworkCore.Entities.SpeciesEntity", "Species")
                        .WithMany("RegionalNumbers")
                        .HasForeignKey("SpeciesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Region");

                    b.Navigation("Species");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.VarietyEntity", b =>
                {
                    b.HasOne("PokeGame.EntityFrameworkCore.Entities.SpeciesEntity", "Species")
                        .WithMany("Varieties")
                        .HasForeignKey("SpeciesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Species");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.AbilityEntity", b =>
                {
                    b.Navigation("Forms");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.FormEntity", b =>
                {
                    b.Navigation("Abilities");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.RegionEntity", b =>
                {
                    b.Navigation("RegionalNumbers");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.SpeciesEntity", b =>
                {
                    b.Navigation("RegionalNumbers");

                    b.Navigation("Varieties");
                });

            modelBuilder.Entity("PokeGame.EntityFrameworkCore.Entities.VarietyEntity", b =>
                {
                    b.Navigation("Forms");
                });
#pragma warning restore 612, 618
        }
    }
}
