using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Pokemons
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Pokemon), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(PokemonEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(PokemonEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(PokemonEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(PokemonEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(PokemonEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(PokemonEntity.Version), Table);

  public static readonly ColumnId AbilitySlot = new(nameof(PokemonEntity.AbilitySlot), Table);
  public static readonly ColumnId EffortValueAttack = new(nameof(PokemonEntity.EffortValueAttack), Table);
  public static readonly ColumnId EffortValueDefense = new(nameof(PokemonEntity.EffortValueDefense), Table);
  public static readonly ColumnId EffortValueHp = new(nameof(PokemonEntity.EffortValueHp), Table);
  public static readonly ColumnId EffortValueSpecialAttack = new(nameof(PokemonEntity.EffortValueSpecialAttack), Table);
  public static readonly ColumnId EffortValueSpecialDefense = new(nameof(PokemonEntity.EffortValueSpecialDefense), Table);
  public static readonly ColumnId EffortValueSpeed = new(nameof(PokemonEntity.EffortValueSpeed), Table);
  public static readonly ColumnId Experience = new(nameof(PokemonEntity.Experience), Table);
  public static readonly ColumnId FormId = new(nameof(PokemonEntity.FormId), Table);
  public static readonly ColumnId FormUid = new(nameof(PokemonEntity.FormUid), Table);
  public static readonly ColumnId Friendship = new(nameof(PokemonEntity.Friendship), Table);
  public static readonly ColumnId Gender = new(nameof(PokemonEntity.Gender), Table);
  public static readonly ColumnId Height = new(nameof(PokemonEntity.Height), Table);
  public static readonly ColumnId Id = new(nameof(PokemonEntity.Id), Table);
  public static readonly ColumnId IndividualValueAttack = new(nameof(PokemonEntity.IndividualValueAttack), Table);
  public static readonly ColumnId IndividualValueDefense = new(nameof(PokemonEntity.IndividualValueDefense), Table);
  public static readonly ColumnId IndividualValueHp = new(nameof(PokemonEntity.IndividualValueHp), Table);
  public static readonly ColumnId IndividualValueSpecialAttack = new(nameof(PokemonEntity.IndividualValueSpecialAttack), Table);
  public static readonly ColumnId IndividualValueSpecialDefense = new(nameof(PokemonEntity.IndividualValueSpecialDefense), Table);
  public static readonly ColumnId IndividualValueSpeed = new(nameof(PokemonEntity.IndividualValueSpeed), Table);
  public static readonly ColumnId Nature = new(nameof(PokemonEntity.Nature), Table);
  public static readonly ColumnId Nickname = new(nameof(PokemonEntity.Nickname), Table);
  public static readonly ColumnId Notes = new(nameof(PokemonEntity.Notes), Table);
  public static readonly ColumnId PokemonId = new(nameof(PokemonEntity.PokemonId), Table);
  public static readonly ColumnId SpeciesId = new(nameof(PokemonEntity.SpeciesId), Table);
  public static readonly ColumnId SpeciesUid = new(nameof(PokemonEntity.SpeciesUid), Table);
  public static readonly ColumnId Sprite = new(nameof(PokemonEntity.Sprite), Table);
  public static readonly ColumnId Stamina = new(nameof(PokemonEntity.Stamina), Table);
  public static readonly ColumnId TeraType = new(nameof(PokemonEntity.TeraType), Table);
  public static readonly ColumnId UniqueName = new(nameof(PokemonEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(PokemonEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId Url = new(nameof(PokemonEntity.Url), Table);
  public static readonly ColumnId VarietyId = new(nameof(PokemonEntity.VarietyId), Table);
  public static readonly ColumnId VarietyUid = new(nameof(PokemonEntity.VarietyUid), Table);
  public static readonly ColumnId Vitality = new(nameof(PokemonEntity.Vitality), Table);
  public static readonly ColumnId Weight = new(nameof(PokemonEntity.Weight), Table);
}
