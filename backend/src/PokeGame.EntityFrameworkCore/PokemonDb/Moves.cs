using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Moves
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Moves), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(MoveEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(MoveEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(MoveEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(MoveEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(MoveEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(MoveEntity.Version), Table);

  public static readonly ColumnId Accuracy = new(nameof(MoveEntity.Accuracy), Table);
  public static readonly ColumnId Category = new(nameof(MoveEntity.Category), Table);
  public static readonly ColumnId Description = new(nameof(MoveEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(MoveEntity.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(MoveEntity.Id), Table);
  public static readonly ColumnId MoveId = new(nameof(MoveEntity.MoveId), Table);
  public static readonly ColumnId Notes = new(nameof(MoveEntity.Notes), Table);
  public static readonly ColumnId Power = new(nameof(MoveEntity.Power), Table);
  public static readonly ColumnId PowerPoints = new(nameof(MoveEntity.PowerPoints), Table);
  public static readonly ColumnId Type = new(nameof(MoveEntity.Type), Table);
  public static readonly ColumnId UniqueName = new(nameof(MoveEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(MoveEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId Url = new(nameof(MoveEntity.Url), Table);
}
