using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Inventory
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Inventory), alias: null);

  public static readonly ColumnId ItemId = new(nameof(InventoryEntity.ItemId), Table);
  public static readonly ColumnId ItemUid = new(nameof(InventoryEntity.ItemUid), Table);
  public static readonly ColumnId Quantity = new(nameof(InventoryEntity.Quantity), Table);
  public static readonly ColumnId TrainerId = new(nameof(InventoryEntity.TrainerId), Table);
  public static readonly ColumnId TrainerUid = new(nameof(InventoryEntity.TrainerUid), Table);
}
