using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class VarietyMoveConfiguration : IEntityTypeConfiguration<VarietyMoveEntity>
{
  public void Configure(EntityTypeBuilder<VarietyMoveEntity> builder)
  {
    builder.ToTable(PokemonDb.VarietyMoves.Table.Table!, PokemonDb.VarietyMoves.Table.Schema);
    builder.HasKey(x => new { x.VarietyId, x.MoveId });

    builder.HasIndex(x => new { x.VarietyUid, x.MoveUid }).IsUnique();
    builder.HasIndex(x => x.MoveUid);
    builder.HasIndex(x => x.Level);

    builder.HasOne(x => x.Variety).WithMany(x => x.Moves).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Move).WithMany(x => x.Varieties).OnDelete(DeleteBehavior.Cascade);
  }
}
