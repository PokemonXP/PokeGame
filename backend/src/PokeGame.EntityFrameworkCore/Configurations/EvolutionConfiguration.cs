using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class EvolutionConfiguration : AggregateConfiguration<EvolutionEntity>, IEntityTypeConfiguration<EvolutionEntity>
{
  public override void Configure(EntityTypeBuilder<EvolutionEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Evolutions.Table.Table!, PokemonDb.Evolutions.Table.Schema);
    builder.HasKey(x => x.EvolutionId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.SourceUid);
    builder.HasIndex(x => x.TargetUid);
    builder.HasIndex(x => x.ItemUid);
    builder.HasIndex(x => x.Level);
    builder.HasIndex(x => x.HeldItemUid);
    builder.HasIndex(x => x.KnownMoveUid);

    builder.Property(x => x.Trigger).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<EvolutionTrigger>());
    builder.Property(x => x.Gender).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<PokemonGender>());
    builder.Property(x => x.Location).HasMaxLength(Location.MaximumLength);
    builder.Property(x => x.TimeOfDay).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<TimeOfDay>());

    // TODO(fpion): relationships
  }
}
