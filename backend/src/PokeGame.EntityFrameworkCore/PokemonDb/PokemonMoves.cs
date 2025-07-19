using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class PokemonMoves
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.PokemonMoves), alias: null);

  public static readonly ColumnId CurrentPowerPoints = new(nameof(PokemonMoveEntity.CurrentPowerPoints), Table);
  public static readonly ColumnId IsMastered = new(nameof(PokemonMoveEntity.IsMastered), Table);
  public static readonly ColumnId ItemId = new(nameof(PokemonMoveEntity.ItemId), Table);
  public static readonly ColumnId ItemUid = new(nameof(PokemonMoveEntity.ItemUid), Table);
  public static readonly ColumnId Level = new(nameof(PokemonMoveEntity.Level), Table);
  public static readonly ColumnId MaximumPowerPoints = new(nameof(PokemonMoveEntity.MaximumPowerPoints), Table);
  public static readonly ColumnId Method = new(nameof(PokemonMoveEntity.Method), Table);
  public static readonly ColumnId MoveId = new(nameof(PokemonMoveEntity.MoveId), Table);
  public static readonly ColumnId MoveUid = new(nameof(PokemonMoveEntity.MoveUid), Table);
  public static readonly ColumnId Notes = new(nameof(PokemonMoveEntity.Notes), Table);
  public static readonly ColumnId PokemonId = new(nameof(PokemonMoveEntity.PokemonId), Table);
  public static readonly ColumnId PokemonUid = new(nameof(PokemonMoveEntity.PokemonUid), Table);
  public static readonly ColumnId Position = new(nameof(PokemonMoveEntity.Position), Table);
  public static readonly ColumnId ReferencePowerPoints = new(nameof(PokemonMoveEntity.ReferencePowerPoints), Table);
}
