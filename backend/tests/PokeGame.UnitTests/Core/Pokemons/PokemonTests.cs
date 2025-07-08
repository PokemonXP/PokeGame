using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonTests
{
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;

  [Fact(DisplayName = "It should assign the correct characteristic.")]
  public void Given_IVsAndSize_When_ctor_Then_CorrectCharacteristic()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26));
    Assert.Equal("Nods off a lot", pokemon.Characteristic.Text);
  }

  [Fact(DisplayName = "It should calculate the correct experience to next level.")]
  public void Given_Experience_When_ToNextLevel_Then_Correct()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028);
    Assert.Equal(549, pokemon.ToNextLevel);
  }

  [Fact(DisplayName = "It should calculate the correct level.")]
  public void Given_Experience_When_Level_Then_Correct()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028);
    Assert.Equal(21, pokemon.Level);
  }

  [Fact(DisplayName = "It should calculate the correct maximum experience.")]
  public void Given_Experience_When_MaximumExperience_Then_Correct()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028);
    Assert.Equal(7577, pokemon.MaximumExperience);
  }

  [Fact(DisplayName = "It should handle held item changes correctly.")]
  public void Given_HeldItem_When_Change_Then_CorrectChanges()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55));
    Assert.Null(pokemon.HeldItemId);
    pokemon.ClearChanges();

    pokemon.RemoveItem();
    Assert.Null(pokemon.HeldItemId);
    Assert.False(pokemon.HasChanges);

    ItemId itemId = ItemId.NewId();
    ActorId actorId = ActorId.NewId();
    pokemon.HoldItem(itemId, actorId);
    Assert.Equal(itemId, pokemon.HeldItemId);
    Assert.Contains(pokemon.Changes, change => change is PokemonItemHeld held && held.ItemId == itemId && held.ActorId == actorId);

    pokemon.ClearChanges();
    pokemon.HoldItem(itemId, actorId);
    Assert.Equal(itemId, pokemon.HeldItemId);
    Assert.False(pokemon.HasChanges);

    pokemon.RemoveItem(actorId);
    Assert.Null(pokemon.HeldItemId);
    Assert.Contains(pokemon.Changes, change => change is PokemonItemRemoved removed && removed.ActorId == actorId);
  }

  [Fact(DisplayName = "It should not create a Pokémon with more Stamina than its maximum.")]
  public void Given_StaminaGreaterThanMaximum_When_ctor_Then_MaximumStamina()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028,
      vitality: 64,
      stamina: 999);
    Assert.Equal(64, pokemon.Vitality);
    Assert.Equal(pokemon.Statistics.HP, pokemon.Stamina);
  }

  [Fact(DisplayName = "It should not create a Pokémon with more Vitality than its maximum.")]
  public void Given_VitalityGreaterThanMaximum_When_ctor_Then_MaximumVitality()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028,
      vitality: int.MaxValue);
    Assert.Equal(pokemon.Statistics.HP, pokemon.Vitality);
  }

  [Fact(DisplayName = "It should not have the correct flavor preferrences.")]
  public void Given_Nature_When_Flavors_Then_CorrectFlavors()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55));
    Assert.Equal(Flavor.Bitter, pokemon.FavoriteFlavor);
    Assert.Equal(Flavor.Dry, pokemon.DislikedFlavor);
  }

  [Theory(DisplayName = "LearnMove: it should add the move to the list when the Pokémon has not reached the move limit.")]
  [InlineData(null)]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void Given_MoveLimitNotReached_When_LearnMove_Then_MoveAdded(int? position)
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "rowlet"),
      PokemonType.Grass,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Gentle,
      new BaseStatistics(60, 55, 55, 50, 50, 42),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(24, 31, 21, 25, 22, 23),
      new EffortValues(),
      GrowthRate.MediumSlow,
      experience: 179,
      vitality: int.MaxValue,
      stamina: int.MaxValue,
      friendship: 70);
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(35)); // NOTE(fpion): Tackle
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Growl
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Leafage

    MoveId moveId = MoveId.NewId(); // NOTE(fpion): Astonish
    PowerPoints powerPoints = new(15);
    ActorId actorId = ActorId.NewId();
    Assert.True(pokemon.LearnMove(moveId, powerPoints, position, actorId));

    Assert.Contains(pokemon.Changes, change => change is PokemonMoveLearned learned && learned.ActorId == actorId
      && learned.PowerPoints == powerPoints && learned.Position == null && !learned.TechnicalMachine);

    PokemonMove move = new(moveId, powerPoints.Value, powerPoints.Value, powerPoints, IsMastered: false, pokemon.Level, TechnicalMachine: false);
    Assert.Equal(move, pokemon.AllMoves[move.MoveId]);
    Assert.Equal(Pokemon.MoveLimit, pokemon.Moves.Count);
    Assert.Equal(move, pokemon.Moves.Last());
  }

  [Theory(DisplayName = "LearnMove: it should replace a move in the list when the Pokémon has reached the move limit.")]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void Given_MoveLimitReached_When_LearnMove_Then_MoveReplaced(int position)
  {
    MoveId tackle = MoveId.NewId();
    MoveId growl = MoveId.NewId();
    MoveId leafage = MoveId.NewId();
    MoveId astonish = MoveId.NewId();
    MoveId peck = MoveId.NewId();

    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "rowlet"),
      PokemonType.Grass,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Gentle,
      new BaseStatistics(60, 55, 55, 50, 50, 42),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(24, 31, 21, 25, 22, 23),
      new EffortValues(),
      GrowthRate.MediumSlow,
      experience: 419,
      vitality: int.MaxValue,
      stamina: int.MaxValue,
      friendship: 70);
    pokemon.LearnMove(tackle, new PowerPoints(35));
    pokemon.LearnMove(growl, new PowerPoints(40));
    pokemon.LearnMove(leafage, new PowerPoints(40));
    pokemon.LearnMove(astonish, new PowerPoints(15));

    PowerPoints powerPoints = new(40);
    ActorId actorId = ActorId.NewId();
    Assert.True(pokemon.LearnMove(peck, powerPoints, position, actorId));

    Assert.Contains(pokemon.Changes, change => change is PokemonMoveLearned learned && learned.ActorId == actorId
      && learned.PowerPoints == powerPoints && learned.Position == position && !learned.TechnicalMachine);

    PokemonMove move = new(peck, powerPoints.Value, powerPoints.Value, powerPoints, IsMastered: false, pokemon.Level, TechnicalMachine: false);
    Assert.Equal(move, pokemon.AllMoves[move.MoveId]);
    Assert.Equal(Pokemon.MoveLimit, pokemon.Moves.Count);
    Assert.Equal(move, pokemon.Moves.ElementAt(position));

    Assert.Equal(5, pokemon.AllMoves.Count);
    Assert.True(pokemon.AllMoves.ContainsKey(tackle));
    Assert.True(pokemon.AllMoves.ContainsKey(growl));
    Assert.True(pokemon.AllMoves.ContainsKey(leafage));
    Assert.True(pokemon.AllMoves.ContainsKey(astonish));
    Assert.True(pokemon.AllMoves.ContainsKey(peck));
  }

  [Fact(DisplayName = "LearnMove: it should return false when the Pokémon has already learned the move.")]
  public void Given_MoveAlreadyLearned_When_LearnMove_Then_FalseReturned()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "rowlet"),
      PokemonType.Grass,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Gentle,
      new BaseStatistics(60, 55, 55, 50, 50, 42),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(24, 31, 21, 25, 22, 23),
      new EffortValues(),
      GrowthRate.MediumSlow,
      experience: 135,
      vitality: int.MaxValue,
      stamina: int.MaxValue,
      friendship: 70);
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    pokemon.LearnMove(moveId, powerPoints);
    Assert.False(pokemon.LearnMove(moveId, powerPoints));
  }

  [Fact(DisplayName = "LearnMove: it should throw ArgumentNullException when the position is null and the Pokémon has reached the move limit.")]
  public void Given_MoveLimitReachedPositionNull_When_LearnMove_Then_ArgumentNullException()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "rowlet"),
      PokemonType.Grass,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Gentle,
      new BaseStatistics(60, 55, 55, 50, 50, 42),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(24, 31, 21, 25, 22, 23),
      new EffortValues(),
      GrowthRate.MediumSlow,
      experience: 419,
      vitality: int.MaxValue,
      stamina: int.MaxValue,
      friendship: 70);
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(35)); // NOTE(fpion): Tackle
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Growl
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Leafage
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(15)); // NOTE(fpion): Astonish
    var exception = Assert.Throws<ArgumentNullException>(() => pokemon.LearnMove(MoveId.NewId(), new PowerPoints(35))); // NOTE(fpion): Peck
    Assert.Equal("position", exception.ParamName);
  }

  [Theory(DisplayName = "LearnMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_LearnMove_Then_ArgumentOutOfRangeException(int position)
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "rowlet"),
      PokemonType.Grass,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Gentle,
      new BaseStatistics(60, 55, 55, 50, 50, 42),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(24, 31, 21, 25, 22, 23),
      new EffortValues(),
      GrowthRate.MediumSlow,
      experience: 135,
      vitality: int.MaxValue,
      stamina: int.MaxValue,
      friendship: 70);
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => pokemon.LearnMove(moveId, powerPoints, position));
    Assert.Equal("position", exception.ParamName);
  }
}
