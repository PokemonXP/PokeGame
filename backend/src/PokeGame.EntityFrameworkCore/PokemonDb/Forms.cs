using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Forms
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Forms), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(FormEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(FormEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(FormEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(FormEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(FormEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(FormEntity.Version), Table);

  public static readonly ColumnId BaseAttack = new(nameof(FormEntity.BaseAttack), Table);
  public static readonly ColumnId BaseDefense = new(nameof(FormEntity.BaseDefense), Table);
  public static readonly ColumnId BaseHP = new(nameof(FormEntity.BaseHP), Table);
  public static readonly ColumnId BaseSpecialAttack = new(nameof(FormEntity.BaseSpecialAttack), Table);
  public static readonly ColumnId BaseSpecialDefense = new(nameof(FormEntity.BaseSpecialDefense), Table);
  public static readonly ColumnId BaseSpeed = new(nameof(FormEntity.BaseSpeed), Table);
  public static readonly ColumnId Description = new(nameof(FormEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(FormEntity.DisplayName), Table);
  public static readonly ColumnId FormId = new(nameof(FormEntity.FormId), Table);
  public static readonly ColumnId Height = new(nameof(FormEntity.Height), Table);
  public static readonly ColumnId Id = new(nameof(FormEntity.Id), Table);
  public static readonly ColumnId IsBattleOnly = new(nameof(FormEntity.IsBattleOnly), Table);
  public static readonly ColumnId IsDefault = new(nameof(FormEntity.IsDefault), Table);
  public static readonly ColumnId IsMega = new(nameof(FormEntity.IsMega), Table);
  public static readonly ColumnId Notes = new(nameof(FormEntity.Notes), Table);
  public static readonly ColumnId PrimaryType = new(nameof(FormEntity.PrimaryType), Table);
  public static readonly ColumnId SecondaryType = new(nameof(FormEntity.SecondaryType), Table);
  public static readonly ColumnId SpriteAlternative = new(nameof(FormEntity.SpriteAlternative), Table);
  public static readonly ColumnId SpriteAlternativeShiny = new(nameof(FormEntity.SpriteAlternativeShiny), Table);
  public static readonly ColumnId SpriteDefault = new(nameof(FormEntity.SpriteDefault), Table);
  public static readonly ColumnId SpriteShiny = new(nameof(FormEntity.SpriteShiny), Table);
  public static readonly ColumnId UniqueName = new(nameof(FormEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(FormEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId Url = new(nameof(FormEntity.Url), Table);
  public static readonly ColumnId VarietyId = new(nameof(FormEntity.VarietyId), Table);
  public static readonly ColumnId VarietyUid = new(nameof(FormEntity.VarietyUid), Table);
  public static readonly ColumnId Weight = new(nameof(FormEntity.Weight), Table);
  public static readonly ColumnId YieldAttack = new(nameof(FormEntity.YieldAttack), Table);
  public static readonly ColumnId YieldDefense = new(nameof(FormEntity.YieldDefense), Table);
  public static readonly ColumnId YieldExperience = new(nameof(FormEntity.YieldExperience), Table);
  public static readonly ColumnId YieldHP = new(nameof(FormEntity.YieldHP), Table);
  public static readonly ColumnId YieldSpecialAttack = new(nameof(FormEntity.YieldSpecialAttack), Table);
  public static readonly ColumnId YieldSpecialDefense = new(nameof(FormEntity.YieldSpecialDefense), Table);
  public static readonly ColumnId YieldSpeed = new(nameof(FormEntity.YieldSpeed), Table);
}
