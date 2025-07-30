using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class BattleTrainerConfiguration : IEntityTypeConfiguration<BattleTrainerEntity>
{
  public void Configure(EntityTypeBuilder<BattleTrainerEntity> builder)
  {
    builder.ToTable(PokemonDb.BattleTrainers.Table.Table!, PokemonDb.BattleTrainers.Table.Schema);
    builder.HasKey(x => new { x.BattleId, x.TrainerId });

    builder.HasIndex(x => new { x.BattleUid, x.TrainerUid }).IsUnique();
    builder.HasIndex(x => x.TrainerUid);
    builder.HasIndex(x => x.IsOpponent);

    builder.HasOne(x => x.Battle).WithMany(x => x.Trainers).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Trainer).WithMany(x => x.Battles).OnDelete(DeleteBehavior.Cascade);
  }
}
