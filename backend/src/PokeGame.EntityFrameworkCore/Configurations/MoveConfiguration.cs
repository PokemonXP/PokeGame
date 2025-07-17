using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class MoveConfiguration : AggregateConfiguration<MoveEntity>, IEntityTypeConfiguration<MoveEntity>
{
  public override void Configure(EntityTypeBuilder<MoveEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Moves.Table.Table!, PokemonDb.Moves.Table.Schema);
    builder.HasKey(x => x.MoveId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Type);
    builder.HasIndex(x => x.Category);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Accuracy);
    builder.HasIndex(x => x.Power);
    builder.HasIndex(x => x.PowerPoints);

    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<PokemonType>());
    builder.Property(x => x.Category).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<MoveCategory>());
    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);
  }
}
