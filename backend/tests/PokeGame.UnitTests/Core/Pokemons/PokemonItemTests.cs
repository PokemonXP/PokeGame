using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonItemTests
{
  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Pokemon2 _pokemon;
  private readonly Item _item;

  public PokemonItemTests()
  {
    UniqueNameSettings uniqueNameSettings = new();

    _species = new PokemonSpecies(number: 499, new UniqueName(uniqueNameSettings, "pignite"),
      new CatchRate(45), PokemonCategory.Standard, new Friendship(70), GrowthRate.MediumSlow);

    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(7));

    Ability blaze = new(new UniqueName(uniqueNameSettings, "blaze"));
    Ability thickFat = new(new UniqueName(uniqueNameSettings, "thick-fat"));
    _form = new Form(_variety, _variety.UniqueName, new Types(PokemonType.Fire, PokemonType.Fighting),
      new FormAbilities(blaze, secondary: null, thickFat), new BaseStatistics(90, 93, 55, 70, 55, 55), isDefault: true);

    _pokemon = new Pokemon2(
      _species,
      _variety,
      _form,
      new UniqueName(uniqueNameSettings, "briquet"),
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Find("careful"),
      new IndividualValues(),
      PokemonGender.Male);

    _item = new(new UniqueName(uniqueNameSettings, "leftovers"), ItemCategory.OtherItem);
  }

  [Fact(DisplayName = "HoldItem: it should handle held item changes correctly.")]
  public void Given_Item_When_HoldItem_Then_ItemHeld()
  {
    ActorId actorId = ActorId.NewId();
    Assert.Null(_pokemon.HeldItemId);

    _pokemon.ClearChanges();
    _pokemon.RemoveItem();
    Assert.Null(_pokemon.HeldItemId);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);

    _pokemon.HoldItem(_item, actorId);
    Assert.Equal(_item.Id, _pokemon.HeldItemId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonItemHeld held && held.ItemId == _item.Id && held.ActorId == actorId);

    _pokemon.ClearChanges();
    _pokemon.HoldItem(_item.Id, actorId);
    Assert.Equal(_item.Id, _pokemon.HeldItemId);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "RemoveItem: it should remove the held item.")]
  public void Given_Item_When_RemoveItem_Then_Removed()
  {
    ActorId actorId = ActorId.NewId();

    _pokemon.HoldItem(_item);
    Assert.NotNull(_pokemon.HeldItemId);

    _pokemon.RemoveItem(actorId);
    Assert.Null(_pokemon.HeldItemId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonItemRemoved removed && removed.ActorId == actorId);

    _pokemon.ClearChanges();
    _pokemon.RemoveItem();
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }
}
