using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class VarietyConfiguration : AggregateConfiguration<VarietyEntity>, IEntityTypeConfiguration<VarietyEntity>
{
  public override void Configure(EntityTypeBuilder<VarietyEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Varieties.Table.Table!, PokemonDb.Varieties.Table.Schema);
    builder.HasKey(x => x.VarietyId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.SpeciesUid);
    builder.HasIndex(x => new { x.SpeciesId, x.IsDefault });
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Genus);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Genus).HasMaxLength(16);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);
  }
}
