using Krakenar.Core;
using Krakenar.Core.Settings;
using PokeGame.Core.Abilities;
using PokeGame.Core.Abilities.Models;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Models;
using PokeGame.Core.Varieties;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core;

internal static class ModelExtensions
{
  public static Ability ToAbility(this AbilityModel model)
  {
    Ability ability = new(
      new UniqueName(new UniqueNameSettings(), model.UniqueName),
      actorId: null,
      new AbilityId(model.Id))
    {
      DisplayName = DisplayName.TryCreate(model.DisplayName),
      Description = Description.TryCreate(model.Description),
      Url = Url.TryCreate(model.Url),
      Notes = Notes.TryCreate(model.Notes)
    };
    return ability;
  }

  public static Form ToForm(this FormModel model, Variety variety)
  {
    Ability primary = ToAbility(model.Abilities.Primary);
    Ability? secondary = model.Abilities.Secondary is null ? null : ToAbility(model.Abilities.Secondary);
    Ability? hidden = model.Abilities.Hidden is null ? null : ToAbility(model.Abilities.Hidden);

    Form form = new(
      variety,
      new UniqueName(new UniqueNameSettings(), model.UniqueName),
      new Types(model.Types),
      new FormAbilities(primary, secondary, hidden),
      new BaseStatistics(model.BaseStatistics),
      model.IsDefault,
      actorId: null,
      new FormId(model.Id))
    {
      DisplayName = DisplayName.TryCreate(model.DisplayName),
      Description = Description.TryCreate(model.Description),
      IsBattleOnly = model.IsBattleOnly,
      IsMega = model.IsMega,
      Height = model.Height,
      Weight = model.Weight,
      Url = Url.TryCreate(model.Url),
      Notes = Notes.TryCreate(model.Notes)
    };
    return form;
  }

  public static Item ToItem(this ItemModel model)
  {
    Item item = new(new UniqueName(new UniqueNameSettings(), model.UniqueName), actorId: null, new ItemId(model.Id))
    {
      DisplayName = DisplayName.TryCreate(model.DisplayName),
      Description = Description.TryCreate(model.Description)
    };
    return item;
  }

  public static PokemonSpecies ToPokemonSpecies(this SpeciesModel model)
  {
    PokemonSpecies species = new(
      model.Number,
      new UniqueName(new UniqueNameSettings(), model.UniqueName),
      new CatchRate(model.CatchRate),
      model.Category,
      new Friendship(model.BaseFriendship),
      model.GrowthRate,
      actorId: null,
      new SpeciesId(model.Id))
    {
      DisplayName = DisplayName.TryCreate(model.DisplayName),
      Url = Url.TryCreate(model.Url),
      Notes = Notes.TryCreate(model.Notes)
    };
    return species;
  }

  public static Variety ToVariety(this VarietyModel model, PokemonSpecies species)
  {
    Variety variety = new(
      species,
      new UniqueName(new UniqueNameSettings(), model.UniqueName),
      model.IsDefault,
      model.GenderRatio.HasValue ? new GenderRatio(model.GenderRatio.Value) : null,
      model.CanChangeForm,
      actorId: null,
      new VarietyId(model.Id))
    {
      DisplayName = DisplayName.TryCreate(model.DisplayName),
      Genus = Genus.TryCreate(model.Genus),
      Description = Description.TryCreate(model.Description),
      Url = Url.TryCreate(model.Url),
      Notes = Notes.TryCreate(model.Notes)
    };
    return variety;
  }
}
