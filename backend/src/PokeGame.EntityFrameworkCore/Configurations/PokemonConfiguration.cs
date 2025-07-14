using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Species;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class PokemonConfiguration : AggregateConfiguration<PokemonEntity>, IEntityTypeConfiguration<PokemonEntity>
{
  public override void Configure(EntityTypeBuilder<PokemonEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Pokemons.Table.Table!, PokemonDb.Pokemons.Table.Schema);
    builder.HasKey(x => x.PokemonId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.SpeciesUid);
    builder.HasIndex(x => x.VarietyUid);
    builder.HasIndex(x => x.FormUid);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.Nickname);
    builder.HasIndex(x => x.Gender);
    builder.HasIndex(x => x.TeraType);
    builder.HasIndex(x => x.Nature);
    builder.HasIndex(x => x.Level);
    builder.HasIndex(x => x.Experience);
    builder.HasIndex(x => x.StatusCondition);
    builder.HasIndex(x => x.Friendship);
    builder.HasIndex(x => x.HeldItemUid);
    builder.HasIndex(x => x.OriginalTrainerUid);
    builder.HasIndex(x => x.CurrentTrainerUid);
    builder.HasIndex(x => x.PokeBallUid);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.Nickname).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Gender).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<PokemonGender>());
    builder.Property(x => x.TeraType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<PokemonType>());
    builder.Property(x => x.AbilitySlot).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<AbilitySlot>());
    builder.Property(x => x.Nature).HasMaxLength(PokemonNature.MaximumLength);
    builder.Property(x => x.GrowthRate).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<GrowthRate>());
    builder.Property(x => x.Statistics).HasMaxLength(Constants.StatisticsMaximumLength);
    builder.Property(x => x.StatusCondition).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<StatusCondition>());
    builder.Property(x => x.Characteristic).HasMaxLength(PokemonCharacteristic.MaximumLength);
    builder.Property(x => x.OwnershipKind).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<OwnershipKind>());
    builder.Property(x => x.MetLocation).HasMaxLength(GameLocation.MaximumLength);
    builder.Property(x => x.Sprite).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.Species).WithMany(x => x.Pokemon).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Variety).WithMany(x => x.Pokemon).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Form).WithMany(x => x.Pokemon).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.HeldItem).WithMany(x => x.HoldingPokemon)
      .HasForeignKey(x => x.HeldItemId).HasPrincipalKey(x => x.ItemId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.OriginalTrainer).WithMany(x => x.OriginalPokemon)
      .HasForeignKey(x => x.OriginalTrainerId).HasPrincipalKey(x => x.TrainerId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.CurrentTrainer).WithMany(x => x.CurrentPokemon)
      .HasForeignKey(x => x.CurrentTrainerId).HasPrincipalKey(x => x.TrainerId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.PokeBall).WithMany(x => x.ContainedPokemon)
      .HasForeignKey(x => x.PokeBallId).HasPrincipalKey(x => x.ItemId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
