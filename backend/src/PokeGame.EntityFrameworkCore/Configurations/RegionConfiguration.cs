using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class RegionConfiguration : AggregateConfiguration<RegionEntity>, IEntityTypeConfiguration<RegionEntity>
{
  public override void Configure(EntityTypeBuilder<RegionEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Regions.Table.Table!, PokemonDb.Regions.Table.Schema);
    builder.HasKey(x => x.RegionId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);
  }
}
