using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class FormAbilities
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.FormAbilities), alias: null);

  public static readonly ColumnId AbilityId = new(nameof(FormAbilityEntity.AbilityId), Table);
  public static readonly ColumnId AbilityUid = new(nameof(FormAbilityEntity.AbilityUid), Table);
  public static readonly ColumnId FormId = new(nameof(FormAbilityEntity.FormId), Table);
  public static readonly ColumnId FormUid = new(nameof(FormAbilityEntity.FormUid), Table);
  public static readonly ColumnId Slot = new(nameof(FormAbilityEntity.Slot), Table);
}
