using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class RegionalNumberConfiguration : IEntityTypeConfiguration<RegionalNumberEntity> // TODO(fpion): next migration
{
  public void Configure(EntityTypeBuilder<RegionalNumberEntity> builder)
  {
    builder.ToTable(PokemonDb.RegionalNumbers.Table.Table!, PokemonDb.RegionalNumbers.Table.Schema);
    builder.HasKey(x => new { x.SpeciesId, x.RegionId });

    builder.HasIndex(x => new { x.RegionId, x.Number }).IsUnique();
    builder.HasIndex(x => x.SpeciesUid);
    builder.HasIndex(x => x.RegionUid);
    //builder.HasIndex(x => new { x.SpeciesUid, x.RegionUid }).IsUnique();
    //builder.HasIndex(x => new { x.RegionUid, x.Number }).IsUnique();

    builder.HasOne(x => x.Species).WithMany(x => x.RegionalNumbers).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Region).WithMany(x => x.RegionalNumbers).OnDelete(DeleteBehavior.Cascade);
  }
}
