using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Battles
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Battles), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(BattleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(BattleEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(BattleEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(BattleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(BattleEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(BattleEntity.Version), Table);

  public static readonly ColumnId BattleId = new(nameof(BattleEntity.BattleId), Table);
  public static readonly ColumnId ChampionCount = new(nameof(BattleEntity.ChampionCount), Table);
  public static readonly ColumnId Id = new(nameof(BattleEntity.Id), Table);
  public static readonly ColumnId Kind = new(nameof(BattleEntity.Kind), Table);
  public static readonly ColumnId Location = new(nameof(BattleEntity.Location), Table);
  public static readonly ColumnId Name = new(nameof(BattleEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(BattleEntity.Notes), Table);
  public static readonly ColumnId OpponentCount = new(nameof(BattleEntity.OpponentCount), Table);
  public static readonly ColumnId Status = new(nameof(BattleEntity.Status), Table);
  public static readonly ColumnId Url = new(nameof(BattleEntity.Url), Table);
}
