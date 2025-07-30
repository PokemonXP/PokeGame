using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core.Battles;
using PokeGame.Core.Regions;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class BattleConfiguration : AggregateConfiguration<BattleEntity>, IEntityTypeConfiguration<BattleEntity>
{
  public override void Configure(EntityTypeBuilder<BattleEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Battles.Table.Table!, PokemonDb.Battles.Table.Schema);
    builder.HasKey(x => x.BattleId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Kind);
    builder.HasIndex(x => x.Status);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Location);

    builder.Property(x => x.Kind).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<BattleKind>());
    builder.Property(x => x.Status).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<BattleStatus>());
    builder.Property(x => x.Name).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Location).HasMaxLength(Location.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);
  }
}
