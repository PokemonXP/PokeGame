using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Varieties
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Varieties), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(VarietyEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(VarietyEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(VarietyEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(VarietyEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(VarietyEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(VarietyEntity.Version), Table);

  public static readonly ColumnId CanChangeForm = new(nameof(VarietyEntity.CanChangeForm), Table);
  public static readonly ColumnId Description = new(nameof(VarietyEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(VarietyEntity.DisplayName), Table);
  public static readonly ColumnId GenderRatio = new(nameof(VarietyEntity.GenderRatio), Table);
  public static readonly ColumnId Genus = new(nameof(VarietyEntity.Genus), Table);
  public static readonly ColumnId Id = new(nameof(VarietyEntity.Id), Table);
  public static readonly ColumnId IsDefault = new(nameof(VarietyEntity.IsDefault), Table);
  public static readonly ColumnId Notes = new(nameof(VarietyEntity.Notes), Table);
  public static readonly ColumnId SpeciesId = new(nameof(VarietyEntity.SpeciesId), Table);
  public static readonly ColumnId SpeciesUid = new(nameof(VarietyEntity.SpeciesUid), Table);
  public static readonly ColumnId UniqueName = new(nameof(VarietyEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(VarietyEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId Url = new(nameof(VarietyEntity.Url), Table);
  public static readonly ColumnId VarietyId = new(nameof(VarietyEntity.VarietyId), Table);
}
