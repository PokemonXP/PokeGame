using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Abilities
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Abilities), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(AbilityEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(AbilityEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(AbilityEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(AbilityEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(AbilityEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(AbilityEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(AbilityEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(AbilityEntity.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(AbilityEntity.Id), Table);
  public static readonly ColumnId Notes = new(nameof(AbilityEntity.Notes), Table);
  public static readonly ColumnId AbilityId = new(nameof(AbilityEntity.AbilityId), Table);
  public static readonly ColumnId UniqueName = new(nameof(AbilityEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(AbilityEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId Url = new(nameof(AbilityEntity.Url), Table);
}
