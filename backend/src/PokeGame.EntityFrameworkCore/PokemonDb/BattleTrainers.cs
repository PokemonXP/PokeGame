using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class BattleTrainers
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.BattleTrainers), alias: null);

  public static readonly ColumnId BattleId = new(nameof(BattleTrainerEntity.BattleId), Table);
  public static readonly ColumnId BattleUid = new(nameof(BattleTrainerEntity.BattleUid), Table);
  public static readonly ColumnId IsOpponent = new(nameof(BattleTrainerEntity.IsOpponent), Table);
  public static readonly ColumnId TrainerId = new(nameof(BattleTrainerEntity.TrainerId), Table);
  public static readonly ColumnId TrainerUid = new(nameof(BattleTrainerEntity.TrainerUid), Table);
}
