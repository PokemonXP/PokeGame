using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Pokemons.Events;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonItemTests
{
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly Pokemon _briquet;

  public PokemonItemTests()
  {
    _briquet = new Pokemon(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55));
  }

  [Fact(DisplayName = "HoldItem: it should handle held item changes correctly.")]
  public void Given_Item_When_HoldItem_Then_ItemHeld()
  {
    Assert.Null(_briquet.HeldItemId);
    _briquet.ClearChanges();

    _briquet.RemoveItem();
    Assert.Null(_briquet.HeldItemId);
    Assert.False(_briquet.HasChanges);

    ItemId itemId = ItemId.NewId();
    ActorId actorId = ActorId.NewId();
    _briquet.HoldItem(itemId, actorId);
    Assert.Equal(itemId, _briquet.HeldItemId);
    Assert.Contains(_briquet.Changes, change => change is PokemonItemHeld held && held.ItemId == itemId && held.ActorId == actorId);

    _briquet.ClearChanges();
    _briquet.HoldItem(itemId, actorId);
    Assert.Equal(itemId, _briquet.HeldItemId);
    Assert.False(_briquet.HasChanges);
  }

  [Fact(DisplayName = "RemoveItem: it should remove the held item.")]
  public void Given_Item_When_RemoveItem_Then_Removed()
  {
    _briquet.HoldItem(ItemId.NewId());
    Assert.NotNull(_briquet.HeldItemId);

    ActorId actorId = ActorId.NewId();
    _briquet.RemoveItem(actorId);
    Assert.Null(_briquet.HeldItemId);
    Assert.Contains(_briquet.Changes, change => change is PokemonItemRemoved removed && removed.ActorId == actorId);

    _briquet.ClearChanges();
    _briquet.RemoveItem();
    Assert.False(_briquet.HasChanges);
  }
}
