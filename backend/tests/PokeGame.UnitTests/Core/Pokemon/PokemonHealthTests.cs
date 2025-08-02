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
public class PokemonHealthTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;

  public PokemonHealthTests()
  {
    _species = new(new Number(722), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "rowlet"),
      new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow, new EggCycles(15), new EggGroups(EggGroup.Flying));

    _variety = new(_species, _species.UniqueName, isDefault: true, new GenderRatio(7));

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

    _pokemon = new Specimen(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!));
  }

  [Fact(DisplayName = "Heal: it should not do anything when the Pokémon is healthy.")]
  public void Given_Healthy_When_Heal_Then_DoNothing()
  {
    _pokemon.ClearChanges();
    _pokemon.Heal(_pokemon.Statistics.HP, StatusCondition.Paralysis, _actorId);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "Heal: it should remove any status condition.")]
  public void Given_AllConditions_When_Heal_Then_StatusRemoved()
  {
    _pokemon.StatusCondition = StatusCondition.Paralysis;
    _pokemon.Update(_actorId);

    _pokemon.Heal(healing: null, allConditions: true, _actorId);
    Assert.Null(_pokemon.StatusCondition);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonHealed healed && healed.ActorId == _actorId && healed.Healing == 0 && healed.StatusCondition);
  }

  [Fact(DisplayName = "Heal: it should remove the status condition.")]
  public void Given_StatusCondition_When_Heal_Then_StatusRemoved()
  {
    _pokemon.StatusCondition = StatusCondition.Paralysis;
    _pokemon.Update(_actorId);

    _pokemon.Heal(healing: null, _pokemon.StatusCondition, _actorId);
    Assert.Null(_pokemon.StatusCondition);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonHealed healed && healed.ActorId == _actorId && healed.Healing == 0 && healed.StatusCondition);
  }

  [Theory(DisplayName = "Heal: it should restore the Pokémon vitality.")]
  [InlineData(1)]
  [InlineData(999)]
  public void Given_Healing_When_Heal_Then_VitalityRestored(int healing)
  {
    Assert.True(healing > 0);

    _pokemon.Vitality = 1;
    _pokemon.Update(_actorId);
    bool full = healing >= (_pokemon.Statistics.HP - _pokemon.Vitality);

    _pokemon.Heal(healing, allConditions: false, _actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonHealed healed && healed.ActorId == _actorId && healed.Healing > 0 && !healed.StatusCondition);
    if (full)
    {
      Assert.Equal(_pokemon.Statistics.HP, _pokemon.Vitality);
    }
    else
    {
      Assert.Equal(1 + healing, _pokemon.Vitality);
    }
  }

  [Fact(DisplayName = "Heal: it should throw ArgumentOutOfRangeException when the healing is negative.")]
  public void Given_HealingNegative_When_Heal_Then_ArgumentOutOfRangeException()
  {
    ArgumentOutOfRangeException exception;

    exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.Heal(healing: -1, allConditions: false));
    Assert.Equal("healing", exception.ParamName);

    exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.Heal(healing: -1, condition: null));
    Assert.Equal("healing", exception.ParamName);
  }

  [Fact(DisplayName = "Heal: it should throw ArgumentOutOfRangeException when the status condition is not defined.")]
  public void Given_ConditionNotDefined_When_Heal_Then_ArgumentOutOfRangeException()
  {
    StatusCondition condition = (StatusCondition)(-1);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.Heal(healing: null, condition));
    Assert.Equal("condition", exception.ParamName);
  }

  [Fact(DisplayName = "Restore: it should fully restore a Pokémon.")]
  public void Given_Pokemon_When_Restore_Then_Restoreed()
  {
    _pokemon.Vitality = 1;
    _pokemon.Stamina = 0;
    _pokemon.StatusCondition = StatusCondition.Paralysis;
    _pokemon.Update();
    Assert.Equal(1, _pokemon.Vitality);
    Assert.Equal(0, _pokemon.Stamina);
    Assert.Equal(StatusCondition.Paralysis, _pokemon.StatusCondition);

    _pokemon.Restore(_actorId);

    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonRestored restoreed && restoreed.ActorId == _actorId);

    int constitution = _pokemon.Statistics.HP;
    Assert.Equal(constitution, _pokemon.Vitality);
    Assert.Equal(constitution, _pokemon.Stamina);
    Assert.Null(_pokemon.StatusCondition);

    foreach (PokemonMove move in _pokemon.LearnedMoves.Values)
    {
      Assert.Equal(move.MaximumPowerPoints, move.CurrentPowerPoints);
    }

    _pokemon.ClearChanges();
    _pokemon.Restore();
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Theory(DisplayName = "Wound: damage should reduce Vitality.")]
  [InlineData(1)]
  [InlineData(999)]
  public void Given_Damage_When_Wound_Then_VitalityReduced(int damage)
  {
    Assert.True(damage > 0);

    int vitality = Math.Max(_pokemon.Vitality - damage, 0);
    bool fainted = vitality <= 0;

    _pokemon.Wound(damage, condition: null, _actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonWounded wounded && wounded.ActorId == _actorId && wounded.Damage > 0 && wounded.StatusCondition == null);
    if (fainted)
    {
      Assert.True(_pokemon.HasFainted);
      Assert.Equal(0, _pokemon.Vitality);
    }
    else
    {
      Assert.Equal(vitality, _pokemon.Vitality);
    }
  }

  [Theory(DisplayName = "Wound: it should inflict a status condition.")]
  [InlineData(StatusCondition.Freeze)]
  public void Given_StatusCondition_When_Wound_Then_Inflicted(StatusCondition statusCondition)
  {
    _pokemon.Wound(damage: null, statusCondition, _actorId);
    Assert.Equal(statusCondition, _pokemon.StatusCondition);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonWounded wounded && wounded.ActorId == _actorId && wounded.Damage == 0 && wounded.StatusCondition == statusCondition);
  }

  [Theory(DisplayName = "Wound: it should not do anything when the Pokémon is already wounded.")]
  [InlineData(StatusCondition.Burn)]
  public void Given_AlreadyWounded_When_Wound_Then_DoNothing(StatusCondition statusCondition)
  {
    _pokemon.StatusCondition = statusCondition;
    _pokemon.Update();

    _pokemon.ClearChanges();
    _pokemon.Wound(damage: null, statusCondition);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);

    _pokemon.Wound(_pokemon.Vitality);
    Assert.True(_pokemon.HasFainted);
    Assert.Null(_pokemon.StatusCondition);

    _pokemon.ClearChanges();
    _pokemon.Wound(damage: null, statusCondition);
    Assert.Null(_pokemon.StatusCondition);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "Wound: it should throw ArgumentOutOfRangeException when the damage is negative.")]
  public void Given_NegativeDamage_When_Wound_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.Wound(damage: -1));
    Assert.Equal("damage", exception.ParamName);
  }

  [Fact(DisplayName = "Wound: it should throw ArgumentOutOfRangeException when the status condition is not defined.")]
  public void Given_ConditionNotDefined_When_Wound_Then_ArgumentOutOfRangeException()
  {
    StatusCondition condition = (StatusCondition)(-1);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.Wound(damage: null, condition));
    Assert.Equal("condition", exception.ParamName);
  }
}
