using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonHealTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _specis;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;

  public PokemonHealTests()
  {
    _specis = new(new Number(722), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "rowlet"),
      new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow, new EggCycles(15), new EggGroups(EggGroup.Flying));

    _variety = new(_specis, _specis.UniqueName, isDefault: true, new GenderRatio(7));

    Ability overgrow = new(new UniqueName(_uniqueNameSettings, "overgrow"));
    Ability longReach = new(new UniqueName(_uniqueNameSettings, "long-reach"));
    _form = new(
      _variety,
      _variety.UniqueName,
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

    _pokemon = new Specimen(_specis, _variety, _form, _specis.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!));
  }

  [Fact(DisplayName = "Heal: it should fully restore a Pokémon.")]
  public void Given_Pokemon_When_Heal_Then_Healed()
  {
    _pokemon.Vitality = 1;
    _pokemon.Stamina = 0;
    _pokemon.StatusCondition = StatusCondition.Paralysis;
    _pokemon.Update();
    Assert.Equal(1, _pokemon.Vitality);
    Assert.Equal(0, _pokemon.Stamina);
    Assert.Equal(StatusCondition.Paralysis, _pokemon.StatusCondition);

    _pokemon.Heal(_actorId);

    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonHealed healed && healed.ActorId == _actorId);

    int constitution = _pokemon.Statistics.HP;
    Assert.Equal(constitution, _pokemon.Vitality);
    Assert.Equal(constitution, _pokemon.Stamina);
    Assert.Null(_pokemon.StatusCondition);

    foreach (PokemonMove move in _pokemon.LearnedMoves.Values)
    {
      Assert.Equal(move.MaximumPowerPoints, move.CurrentPowerPoints);
    }

    _pokemon.ClearChanges();
    _pokemon.Heal();
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }
}
