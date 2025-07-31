using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class BattlePokemonConfiguration : IEntityTypeConfiguration<BattlePokemonEntity>
{
  public void Configure(EntityTypeBuilder<BattlePokemonEntity> builder)
  {
    builder.ToTable(PokemonDb.BattlePokemon.Table.Table!, PokemonDb.BattlePokemon.Table.Schema);
    builder.HasKey(x => new { x.BattleId, x.PokemonId });

    builder.HasIndex(x => new { x.BattleUid, x.PokemonUid }).IsUnique();
    builder.HasIndex(x => x.PokemonUid);
    builder.HasIndex(x => x.IsActive);

    builder.HasOne(x => x.Battle).WithMany(x => x.Pokemon).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Pokemon).WithMany(x => x.Battles).OnDelete(DeleteBehavior.Cascade);
  }
}
