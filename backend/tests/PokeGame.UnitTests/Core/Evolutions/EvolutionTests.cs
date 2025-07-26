using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Evolutions;

[Trait(Traits.Category, Categories.Unit)]
public class EvolutionTests
{
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _rowletSpecies;
  private readonly PokemonSpecies _dartrixSpecies;

  private readonly Variety _rowletVariety;
  private readonly Variety _dartrixVariety;

  private readonly Form _rowletForm;
  private readonly Form _dartrixForm;

  public EvolutionTests()
  {
    _rowletSpecies = new(new Number(722), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "rowlet"),
      new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow, new EggCycles(15), new EggGroups(EggGroup.Flying));
    _dartrixSpecies = new(new Number(723), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "dartrix"),
      new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow, new EggCycles(15), new EggGroups(EggGroup.Flying));

    _rowletVariety = new(_rowletSpecies, _rowletSpecies.UniqueName, isDefault: true, new GenderRatio(7));
    _dartrixVariety = new(_dartrixSpecies, _dartrixSpecies.UniqueName, isDefault: true, new GenderRatio(7));

    Ability overgrow = new(new UniqueName(_uniqueNameSettings, "overgrow"));
    Ability longReach = new(new UniqueName(_uniqueNameSettings, "long-reach"));
    _rowletForm = new(
      _rowletVariety,
      _rowletVariety.UniqueName,
      new FormTypes(PokemonType.Grass, PokemonType.Flying),
      new FormAbilities(overgrow, secondary: null, longReach),
      new BaseStatistics(68, 55, 55, 50, 50, 42),
      new Yield(64, 1, 0, 0, 0, 0, 0),
      new Sprites(
        new Url("https://www.pokegame.com/assets/img/pokemon/rowlet.png"),
        new Url("https://www.pokegame.com/assets/img/pokemon/rowlet-shiny.png")),
      isDefault: true,
      height: new Height(3),
      weight: new Weight(15));
    _dartrixForm = new(
      _dartrixVariety,
      _dartrixVariety.UniqueName,
      new FormTypes(PokemonType.Grass, PokemonType.Flying),
      new FormAbilities(overgrow, secondary: null, longReach),
      new BaseStatistics(78, 75, 75, 70, 70, 52),
      new Yield(147, 2, 0, 0, 0, 0, 0),
      new Sprites(
        new Url("https://www.pokegame.com/assets/img/pokemon/dartrix.png"),
        new Url("https://www.pokegame.com/assets/img/pokemon/dartrix-shiny.png")),
      isDefault: true,
      height: new Height(7),
      weight: new Weight(160));
  }

  [Fact(DisplayName = "It should construct a new evolution.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    ActorId actorId = ActorId.NewId();
    EvolutionId evolutionId = EvolutionId.NewId();
    Evolution evolution = new(_rowletForm, _dartrixForm, EvolutionTrigger.Level, item: null, actorId, evolutionId);

    Assert.Equal(_rowletForm.Id, evolution.SourceId);
    Assert.Equal(_dartrixForm.Id, evolution.TargetId);
    Assert.Equal(EvolutionTrigger.Level, evolution.Trigger);
    Assert.Null(evolution.ItemId);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the forms are the same.")]
  public void Given_SameForm_When_ctor_Then_ArgumentException()
  {
    var exception = Assert.Throws<ArgumentException>(() => new Evolution(_rowletForm, _rowletForm));
    Assert.Equal("target", exception.ParamName);
    Assert.StartsWith("The source and target forms must be different, and be of a different variety.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the forms share the same variety.")]
  public void Given_SameVariety_When_ctor_Then_ArgumentException()
  {
    Form form = new(_rowletVariety, _rowletVariety.UniqueName, _rowletForm.Types, _rowletForm.Abilities,
      _rowletForm.BaseStatistics, _rowletForm.Yield, _rowletForm.Sprites, height: _rowletForm.Height, weight: _rowletForm.Weight);
    var exception = Assert.Throws<ArgumentException>(() => new Evolution(_rowletForm, form));
    Assert.Equal("target", exception.ParamName);
    Assert.StartsWith("The source and target forms must be different, and be of a different variety.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the trigger is not Item, but the item is not null.")]
  public void Given_NotItemTriggerWithItem_When_ctor_Then_ArgumentException()
  {
    Item item = new(new UniqueName(_uniqueNameSettings, "leaf-stone"), new OtherItemProperties());
    var exception = Assert.Throws<ArgumentException>(() => new Evolution(_rowletForm, _dartrixForm, EvolutionTrigger.Level, item));
    Assert.Equal("item", exception.ParamName);
    Assert.StartsWith("The item must be null when the trigger is not 'Item'.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentNullException when the trigger is Item, but the item is null.")]
  public void Given_ItemTriggerWithoutItem_When_ctor_Then_ArgumentNullException()
  {
    var exception = Assert.Throws<ArgumentNullException>(() => new Evolution(_rowletForm, _dartrixForm, EvolutionTrigger.Item));
    Assert.Equal("item", exception.ParamName);
    Assert.StartsWith("The item is required when the trigger is 'Item'.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the trigger is not defined.")]
  public void Given_InvalidTrigger_When_ctor_Then_ArgumentOutOfRangeException()
  {
    EvolutionTrigger trigger = (EvolutionTrigger)(-1);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Evolution(_rowletForm, _dartrixForm, trigger));
    Assert.Equal("trigger", exception.ParamName);
  }
}
