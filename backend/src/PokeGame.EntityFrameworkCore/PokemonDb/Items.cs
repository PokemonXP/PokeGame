using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Items
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Items), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(ItemEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ItemEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(ItemEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ItemEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ItemEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ItemEntity.Version), Table);

  public static readonly ColumnId BattleItem = new(nameof(ItemEntity.BattleItem), Table);
  public static readonly ColumnId Berry = new(nameof(ItemEntity.Berry), Table);
  public static readonly ColumnId Category = new(nameof(ItemEntity.Category), Table);
  public static readonly ColumnId Description = new(nameof(ItemEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(ItemEntity.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(ItemEntity.Id), Table);
  public static readonly ColumnId ItemId = new(nameof(ItemEntity.ItemId), Table);
  public static readonly ColumnId Medicine = new(nameof(ItemEntity.Medicine), Table);
  public static readonly ColumnId MoveId = new(nameof(ItemEntity.MoveId), Table);
  public static readonly ColumnId MoveUid = new(nameof(ItemEntity.MoveUid), Table);
  public static readonly ColumnId Notes = new(nameof(ItemEntity.Notes), Table);
  public static readonly ColumnId PokeBall = new(nameof(ItemEntity.PokeBall), Table);
  public static readonly ColumnId Price = new(nameof(ItemEntity.Price), Table);
  public static readonly ColumnId Sprite = new(nameof(ItemEntity.Sprite), Table);
  public static readonly ColumnId UniqueName = new(nameof(ItemEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(ItemEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId Url = new(nameof(ItemEntity.Url), Table);
}
