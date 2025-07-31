using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Pokemon
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Pokemon), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(PokemonEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(PokemonEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(PokemonEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(PokemonEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(PokemonEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(PokemonEntity.Version), Table);

  public static readonly ColumnId AbilitySlot = new(nameof(PokemonEntity.AbilitySlot), Table);
  public static readonly ColumnId Box = new(nameof(PokemonEntity.Box), Table);
  public static readonly ColumnId Characteristic = new(nameof(PokemonEntity.Characteristic), Table);
  public static readonly ColumnId CurrentTrainerId = new(nameof(PokemonEntity.CurrentTrainerId), Table);
  public static readonly ColumnId CurrentTrainerUid = new(nameof(PokemonEntity.CurrentTrainerUid), Table);
  public static readonly ColumnId EggCycles = new(nameof(PokemonEntity.EggCycles), Table);
  public static readonly ColumnId Experience = new(nameof(PokemonEntity.Experience), Table);
  public static readonly ColumnId FormId = new(nameof(PokemonEntity.FormId), Table);
  public static readonly ColumnId FormUid = new(nameof(PokemonEntity.FormUid), Table);
  public static readonly ColumnId Friendship = new(nameof(PokemonEntity.Friendship), Table);
  public static readonly ColumnId Gender = new(nameof(PokemonEntity.Gender), Table);
  public static readonly ColumnId GrowthRate = new(nameof(PokemonEntity.GrowthRate), Table);
  public static readonly ColumnId Height = new(nameof(PokemonEntity.Height), Table);
  public static readonly ColumnId HeldItemId = new(nameof(PokemonEntity.HeldItemId), Table);
  public static readonly ColumnId HeldItemUid = new(nameof(PokemonEntity.HeldItemUid), Table);
  public static readonly ColumnId Id = new(nameof(PokemonEntity.Id), Table);
  public static readonly ColumnId IsEgg = new(nameof(PokemonEntity.IsEgg), Table);
  public static readonly ColumnId IsShiny = new(nameof(PokemonEntity.IsShiny), Table);
  public static readonly ColumnId Level = new(nameof(PokemonEntity.Level), Table);
  public static readonly ColumnId MaximumExperience = new(nameof(PokemonEntity.MaximumExperience), Table);
  public static readonly ColumnId MetAtLevel = new(nameof(PokemonEntity.MetAtLevel), Table);
  public static readonly ColumnId MetDescription = new(nameof(PokemonEntity.MetDescription), Table);
  public static readonly ColumnId MetLocation = new(nameof(PokemonEntity.MetLocation), Table);
  public static readonly ColumnId MetOn = new(nameof(PokemonEntity.MetOn), Table);
  public static readonly ColumnId Nature = new(nameof(PokemonEntity.Nature), Table);
  public static readonly ColumnId Nickname = new(nameof(PokemonEntity.Nickname), Table);
  public static readonly ColumnId Notes = new(nameof(PokemonEntity.Notes), Table);
  public static readonly ColumnId OriginalTrainerId = new(nameof(PokemonEntity.OriginalTrainerId), Table);
  public static readonly ColumnId OriginalTrainerUid = new(nameof(PokemonEntity.OriginalTrainerUid), Table);
  public static readonly ColumnId OwnershipKind = new(nameof(PokemonEntity.OwnershipKind), Table);
  public static readonly ColumnId PokeBallId = new(nameof(PokemonEntity.PokeBallId), Table);
  public static readonly ColumnId PokeBallUid = new(nameof(PokemonEntity.PokeBallUid), Table);
  public static readonly ColumnId PokemonId = new(nameof(PokemonEntity.PokemonId), Table);
  public static readonly ColumnId Position = new(nameof(PokemonEntity.Position), Table);
  public static readonly ColumnId SpeciesId = new(nameof(PokemonEntity.SpeciesId), Table);
  public static readonly ColumnId SpeciesUid = new(nameof(PokemonEntity.SpeciesUid), Table);
  public static readonly ColumnId Sprite = new(nameof(PokemonEntity.Sprite), Table);
  public static readonly ColumnId Stamina = new(nameof(PokemonEntity.Stamina), Table);
  public static readonly ColumnId Statistics = new(nameof(PokemonEntity.Statistics), Table);
  public static readonly ColumnId StatusCondition = new(nameof(PokemonEntity.StatusCondition), Table);
  public static readonly ColumnId TeraType = new(nameof(PokemonEntity.TeraType), Table);
  public static readonly ColumnId ToNextLevel = new(nameof(PokemonEntity.ToNextLevel), Table);
  public static readonly ColumnId UniqueName = new(nameof(PokemonEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(PokemonEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId Url = new(nameof(PokemonEntity.Url), Table);
  public static readonly ColumnId VarietyId = new(nameof(PokemonEntity.VarietyId), Table);
  public static readonly ColumnId VarietyUid = new(nameof(PokemonEntity.VarietyUid), Table);
  public static readonly ColumnId Vitality = new(nameof(PokemonEntity.Vitality), Table);
  public static readonly ColumnId Weight = new(nameof(PokemonEntity.Weight), Table);
}
