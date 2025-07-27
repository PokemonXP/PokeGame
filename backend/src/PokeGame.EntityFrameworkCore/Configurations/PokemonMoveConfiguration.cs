using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core.Moves;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class PokemonMoveConfiguration : IEntityTypeConfiguration<PokemonMoveEntity>
{
  public void Configure(EntityTypeBuilder<PokemonMoveEntity> builder)
  {
    builder.ToTable(PokemonDb.PokemonMoves.Table.Table!, PokemonDb.PokemonMoves.Table.Schema);
    builder.HasKey(x => new { x.PokemonId, x.MoveId });

    builder.HasIndex(x => new { x.PokemonUid, x.MoveUid }).IsUnique();
    builder.HasIndex(x => x.MoveUid);
    builder.HasIndex(x => x.ItemUid);

    builder.Property(x => x.Method).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<MoveLearningMethod>());

    builder.HasOne(x => x.Pokemon).WithMany(x => x.Moves).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Move).WithMany(x => x.Pokemon).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Item).WithMany(x => x.LearnedMoves).OnDelete(DeleteBehavior.Restrict);
  }
}
