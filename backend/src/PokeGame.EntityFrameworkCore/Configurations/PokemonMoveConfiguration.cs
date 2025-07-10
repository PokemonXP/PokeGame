using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class PokemonMoveConfiguration : IEntityTypeConfiguration<PokemonMoveEntity>
{
  public void Configure(EntityTypeBuilder<PokemonMoveEntity> builder)
  {
    builder.ToTable(PokemonDb.PokemonMoves.Table.Table!, PokemonDb.PokemonMoves.Table.Schema);
    builder.HasKey(x => new { x.PokemonId, x.MoveId });

    builder.HasIndex(x => x.PokemonUid);
    builder.HasIndex(x => x.MoveUid);

    builder.HasOne(x => x.Pokemon).WithMany(x => x.Moves).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Move).WithMany(x => x.Pokemon).OnDelete(DeleteBehavior.Cascade);
  }
}
