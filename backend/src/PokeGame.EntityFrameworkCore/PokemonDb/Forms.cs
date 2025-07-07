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

  public static readonly ColumnId AlternativeSprite = new(nameof(FormEntity.AlternativeSprite), Table);
  public static readonly ColumnId AlternativeSpriteShiny = new(nameof(FormEntity.AlternativeSpriteShiny), Table);
  public static readonly ColumnId AttackBase = new(nameof(FormEntity.AttackBase), Table);
  public static readonly ColumnId AttackYield = new(nameof(FormEntity.AttackYield), Table);
  public static readonly ColumnId DefaultSprite = new(nameof(FormEntity.DefaultSprite), Table);
  public static readonly ColumnId DefaultSpriteShiny = new(nameof(FormEntity.DefaultSpriteShiny), Table);
  public static readonly ColumnId DefenseBase = new(nameof(FormEntity.DefenseBase), Table);
  public static readonly ColumnId DefenseYield = new(nameof(FormEntity.DefenseYield), Table);
  public static readonly ColumnId Description = new(nameof(FormEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(FormEntity.DisplayName), Table);
  public static readonly ColumnId ExperienceYield = new(nameof(FormEntity.ExperienceYield), Table);
  public static readonly ColumnId FormId = new(nameof(FormEntity.FormId), Table);
  public static readonly ColumnId Height = new(nameof(FormEntity.Height), Table);
  public static readonly ColumnId HPBase = new(nameof(FormEntity.HPBase), Table);
  public static readonly ColumnId HPYield = new(nameof(FormEntity.HPYield), Table);
  public static readonly ColumnId Id = new(nameof(FormEntity.Id), Table);
  public static readonly ColumnId IsBattleOnly = new(nameof(FormEntity.IsBattleOnly), Table);
  public static readonly ColumnId IsDefault = new(nameof(FormEntity.IsDefault), Table);
  public static readonly ColumnId IsMega = new(nameof(FormEntity.IsMega), Table);
  public static readonly ColumnId Notes = new(nameof(FormEntity.Notes), Table);
  public static readonly ColumnId PrimaryType = new(nameof(FormEntity.PrimaryType), Table);
  public static readonly ColumnId SecondaryType = new(nameof(FormEntity.SecondaryType), Table);
  public static readonly ColumnId SpecialAttackBase = new(nameof(FormEntity.SpecialAttackBase), Table);
  public static readonly ColumnId SpecialAttackYield = new(nameof(FormEntity.SpecialAttackYield), Table);
  public static readonly ColumnId SpecialDefenseBase = new(nameof(FormEntity.SpecialDefenseBase), Table);
  public static readonly ColumnId SpecialDefenseYield = new(nameof(FormEntity.SpecialDefenseYield), Table);
  public static readonly ColumnId SpeedBase = new(nameof(FormEntity.SpeedBase), Table);
  public static readonly ColumnId SpeedYield = new(nameof(FormEntity.SpeedYield), Table);
  public static readonly ColumnId UniqueName = new(nameof(FormEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(FormEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId Url = new(nameof(FormEntity.Url), Table);
  public static readonly ColumnId VarietyId = new(nameof(FormEntity.VarietyId), Table);
  public static readonly ColumnId VarietyUid = new(nameof(FormEntity.VarietyUid), Table);
  public static readonly ColumnId Weight = new(nameof(FormEntity.Weight), Table);
}
