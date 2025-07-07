using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class RegionalNumberConfiguration : IEntityTypeConfiguration<RegionalNumberEntity>
{
  public void Configure(EntityTypeBuilder<RegionalNumberEntity> builder)
  {
    builder.ToTable(PokemonDb.RegionalNumbers.Table.Table!, PokemonDb.RegionalNumbers.Table.Schema);
    builder.HasKey(x => x.RegionalNumberId);

    builder.HasIndex(x => new { x.SpeciesId, x.RegionId }).IsUnique();
    builder.HasIndex(x => new { x.RegionId, x.Number }).IsUnique();
    builder.HasIndex(x => x.SpeciesUid);
    builder.HasIndex(x => x.RegionUid);

    builder.HasOne(x => x.Species).WithMany(x => x.RegionalNumbers).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Region).WithMany(x => x.RegionalNumbers).OnDelete(DeleteBehavior.Cascade);
  }
}
