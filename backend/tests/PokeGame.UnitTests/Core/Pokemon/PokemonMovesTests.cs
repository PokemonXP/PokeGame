using FluentValidation;
using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonMovesTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;

  private readonly Move _armThrust;
  private readonly Move _defenseCurl;
  private readonly Move _ember;
  private readonly Move _endure;
  private readonly Move _tackle;
  private readonly Move _tailWhip;

  public PokemonMovesTests()
  {
    _species = new PokemonSpecies(new Number(499), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "pignite"), new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow);

    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(7));

    Ability blaze = new(new UniqueName(_uniqueNameSettings, "blaze"));
    Ability thickFat = new(new UniqueName(_uniqueNameSettings, "thick-fat"));
    Sprites sprites = new(new Url("https://www.pokegame.com/assets/img/pokemon/pignite.png"), new Url("https://www.pokegame.com/assets/img/pokemon/pignite-shiny.png"));
    _form = new Form(_variety, _variety.UniqueName, new FormTypes(PokemonType.Fire, PokemonType.Fighting),
      new FormAbilities(blaze, secondary: null, thickFat), new BaseStatistics(90, 93, 55, 70, 55, 55),
      new Yield(146, 0, 2, 0, 0, 0, 0), sprites, isDefault: true, height: new Height(10), weight: new Weight(555));

    _pokemon = new Specimen(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "briquet"), new PokemonSize(128, 128),
      PokemonNatures.Instance.Find("careful"), new IndividualValues(27, 27, 25, 22, 25, 26), PokemonGender.Male, isShiny: false, PokemonType.Fire,
      AbilitySlot.Primary, eggCycles: null, experience: 7028, new EffortValues(4, 0, 16, 0, 0, 16), vitality: 64, stamina: 55, new Friendship(91));

    _armThrust = new Move(PokemonType.Fighting, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "arm-thrust"), new Accuracy(100), new Power(15), new PowerPoints(20));
    _defenseCurl = new Move(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "defense-curl"), accuracy: null, power: null, new PowerPoints(40));
    _ember = new Move(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    _endure = new Move(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "endure"), accuracy: null, power: null, new PowerPoints(10));
    _tackle = new Move(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new Accuracy(100), new Power(40), new PowerPoints(35));
    _tailWhip = new Move(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new Accuracy(100), power: null, new PowerPoints(30));
  }

  [Fact(DisplayName = "LearnMove: it should learn a move by evolving.")]
  public void Given_NoLevel_When_LearnMove_Then_EvolvingMove()
  {
    _pokemon.LearnMove(_tackle, position: 0, new Level(1));
    _pokemon.LearnMove(_tailWhip, position: 1, new Level(1));
    _pokemon.LearnMove(_ember, position: 2, new Level(1));
    _pokemon.LearnMove(_endure, position: 3, new Level(1));

    int position = 3;
    MoveLearningMethod method = MoveLearningMethod.Evolving;
    Notes notes = new("Learned by evolving from Tepig to Pignite.");
    Assert.True(_pokemon.LearnMove(_armThrust, position, level: null, method, notes, _actorId));
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveLearned learned && learned.ActorId == _actorId && learned.MoveId == _armThrust.Id && learned.Position == position);

    KeyValuePair<MoveId, PokemonMove> move = _pokemon.CurrentMoves.ElementAt(position);
    Assert.Equal(_armThrust.Id, move.Key);
    Assert.Equal(_armThrust.PowerPoints.Value, move.Value.CurrentPowerPoints);
    Assert.Equal(_armThrust.PowerPoints.Value, move.Value.MaximumPowerPoints);
    Assert.Equal(_armThrust.PowerPoints, move.Value.ReferencePowerPoints);
    Assert.False(move.Value.IsMastered);
    Assert.Equal(_pokemon.Level, move.Value.Level.Value);
    Assert.Equal(method, move.Value.Method);
    Assert.Null(move.Value.ItemId);
    Assert.Equal(notes, move.Value.Notes);
  }

  [Theory(DisplayName = "LearnMove: it should override the position when the move limit has not been reached.")]
  [InlineData(null)]
  [InlineData(3)]
  public void Given_LimitNotReached_When_LearnMove_Then_PositionOverriden(int? position)
  {
    _pokemon.LearnMove(_tackle, position: 0, new Level(1));
    _pokemon.LearnMove(_tailWhip, position: 1, new Level(1));

    Assert.True(_pokemon.LearnMove(_ember, position, new Level(1)));
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveLearned learned && learned.MoveId == _ember.Id && learned.Position == 2);

    KeyValuePair<MoveId, PokemonMove> move = _pokemon.CurrentMoves.ElementAt(2);
    Assert.Equal(_ember.Id, move.Key);
    Assert.Equal(_ember.PowerPoints.Value, move.Value.CurrentPowerPoints);
    Assert.Equal(_ember.PowerPoints.Value, move.Value.MaximumPowerPoints);
    Assert.Equal(_ember.PowerPoints, move.Value.ReferencePowerPoints);
    Assert.False(move.Value.IsMastered);
    Assert.Equal(1, move.Value.Level.Value);
    Assert.Equal(MoveLearningMethod.LevelingUp, move.Value.Method);
    Assert.Null(move.Value.ItemId);
    Assert.Null(move.Value.Notes);
  }

  [Fact(DisplayName = "LearnMove: it should return false when the Pokémon has already learned the move.")]
  public void Given_MoveAlreadyLearned_When_LearnMove_Then_FalseReturned()
  {
    Assert.True(_pokemon.LearnMove(_tackle));
    Assert.False(_pokemon.LearnMove(_tackle));
  }

  [Theory(DisplayName = "LearnMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_LearnMove_Then_ArgumentOutOfRangeException(int position)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.LearnMove(_tackle, position));
    Assert.Equal("position", exception.ParamName);
  }

  [Fact(DisplayName = "RememberMove: it should remember a forgotten move and return true.")]
  public void Given_ForgottenMove_When_RememberMove_Then_MoveRemembered()
  {
    _pokemon.LearnMove(_tackle);
    _pokemon.LearnMove(_tailWhip);
    _pokemon.LearnMove(_ember);
    _pokemon.LearnMove(_endure);
    _pokemon.LearnMove(_defenseCurl);

    int position = 1;
    Assert.True(_pokemon.RememberMove(_defenseCurl, position, _actorId));
    KeyValuePair<MoveId, PokemonMove> move = _pokemon.CurrentMoves.ElementAt(position);
    Assert.Equal(_defenseCurl.Id, move.Key);

    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveRemembered remembered && remembered.MoveId == _defenseCurl.Id && remembered.Position == position);
  }

  [Fact(DisplayName = "RememberMove: it should return true when the move is currently learned.")]
  public void Given_AlreadyLearned_When_RememberMove_Then_FalseReturned()
  {
    _pokemon.LearnMove(_ember, position: null, new Level(6));
    _pokemon.ClearChanges();

    Assert.True(_pokemon.RememberMove(_ember, position: 0));
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "RememberMove: it should return false when the Pokémon has not learned the move.")]
  public void Given_MoveNeverLearned_When_RememberMove_Then_FalseReturned()
  {
    Assert.False(_pokemon.RememberMove(_ember, position: 0));
  }

  [Theory(DisplayName = "RememberMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_RememberMove_Then_ArgumentOutOfRangeException(int position)
  {
    _pokemon.LearnMove(_tackle);
    _pokemon.LearnMove(_tailWhip);
    _pokemon.LearnMove(_ember);
    _pokemon.LearnMove(_endure);
    _pokemon.LearnMove(_defenseCurl);

    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.RememberMove(_defenseCurl, position));
    Assert.Equal("position", exception.ParamName);
  }

  [Fact(DisplayName = "SwapMoves: it should exchange two moves.")]
  public void Given_SourceAndDestinationMoves_When_SwapMoves_Then_MovesExchanged()
  {
    _pokemon.LearnMove(_tackle);
    _pokemon.LearnMove(_tailWhip);
    _pokemon.LearnMove(_ember);

    int source = 0;
    int destination = 2;
    _pokemon.SwapMoves(source, destination, _actorId);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveSwapped switched && switched.ActorId == _actorId
      && switched.Source == source && switched.Destination == destination);

    Assert.Equal(_ember.Id, _pokemon.CurrentMoves.ElementAt(0).Key);
    Assert.Equal(_tailWhip.Id, _pokemon.CurrentMoves.ElementAt(1).Key);
    Assert.Equal(_tackle.Id, _pokemon.CurrentMoves.ElementAt(2).Key);
  }

  [Fact(DisplayName = "SwapMoves: it should not do anything when the destination move is empty.")]
  public void Given_SourceEqualDestination_When_SwapMoves_Then_NoChange()
  {
    _pokemon.LearnMove(_ember);
    _pokemon.ClearChanges();

    _pokemon.SwapMoves(source: 0, destination: 1);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "SwapMoves: it should not do anything when the source and destination moves are the same.")]
  public void Given_SourceNotCurrent_When_SwapMoves_Then_NoChange()
  {
    _pokemon.LearnMove(_ember);
    _pokemon.ClearChanges();

    _pokemon.SwapMoves(source: 0, destination: 0);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "SwapMoves: it should not do anything when the source move is empty.")]
  public void Given_DestinationNotCurrent_When_SwapMoves_Then_NoChange()
  {
    _pokemon.LearnMove(_ember);
    _pokemon.ClearChanges();

    _pokemon.SwapMoves(source: 1, destination: 0);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Theory(DisplayName = "SwapMoves: it should throw ArgumentOutOfRangeException when the destination is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_DestinationOutOfBounds_When_SwapMoves_Then_ArgumentOutOfRangeException(int destination)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.SwapMoves(source: 0, destination));
    Assert.Equal("destination", exception.ParamName);
  }

  [Theory(DisplayName = "SwapMoves: it should throw ArgumentOutOfRangeException when the source is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_SourceOutOfBounds_When_SwapMoves_Then_ArgumentOutOfRangeException(int source)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.SwapMoves(source, destination: 0));
    Assert.Equal("source", exception.ParamName);
  }

  [Fact(DisplayName = "UseMove: it should throw ArgumentOutOfRangeException when the stamina cost is negative.")]
  public void Given_NegativeStaminaCost_When_UseMove_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.UseMove(_defenseCurl, staminaCost: -1));
    Assert.Equal("staminaCost", exception.ParamName);
  }

  [Fact(DisplayName = "UseMove: it should throw ValidationException when the Pokémon currently does not know the move.")]
  public void Given_MoveNotCurrent_When_UseMove_Then_ValidationException()
  {
    _pokemon.LearnMove(_tackle, position: 0, new Level(1));
    _pokemon.LearnMove(_tailWhip, position: 1, new Level(3));
    _pokemon.LearnMove(_ember, position: 2, new Level(6));
    _pokemon.LearnMove(_endure, position: 3, new Level(9));
    _pokemon.LearnMove(_defenseCurl, position: 3, new Level(13));

    var exception = Assert.Throws<ValidationException>(() => _pokemon.UseMove(_endure));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "MoveId" && e.AttemptedValue?.Equals(_endure.Id.ToGuid()) == true
      && e.ErrorCode == "CurrentMoveValidator" && e.ErrorMessage == "The Pokémon has learned the move, but it does not know it currently." && e.CustomState is not null);
  }

  [Fact(DisplayName = "UseMove: it should throw ValidationException when the Pokémon does not have enough power points.")]
  public void Given_NotEnoughPP_When_UseMove_Then_ValidationException()
  {
    _pokemon.LearnMove(_armThrust, position: 0, level: null, MoveLearningMethod.Evolving);

    PowerPoints powerPointCost = new((byte)(_armThrust.PowerPoints.Value + 1));
    var exception = Assert.Throws<ValidationException>(() => _pokemon.UseMove(_armThrust, powerPointCost));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "PowerPointCost" && e.AttemptedValue?.Equals(powerPointCost.Value) == true
      && e.ErrorCode == "CurrentPowerPointsValidator" && e.ErrorMessage == "The Pokémon does not have enough remaining power points for this move." && e.CustomState is not null);
  }

  [Fact(DisplayName = "UseMove: it should throw ValidationException when the Pokémon has fainted or is unconscious.")]
  public void Given_FaintedOrUnconscious_When_UseMove_Then_ValidationException()
  {
    _pokemon.Vitality = 0;
    _pokemon.Stamina = 0;
    _pokemon.Update();

    var exception = Assert.Throws<ValidationException>(() => _pokemon.UseMove(_ember));
    Assert.True(exception.Errors.Count() >= 2);
    Assert.Contains(exception.Errors, e => e.PropertyName == "PokemonId" && e.AttemptedValue?.Equals(_pokemon.Id.ToGuid()) == true
      && e.ErrorCode == "FaintedValidator" && e.ErrorMessage == "A fainted Pokémon cannot use moves.");
    Assert.Contains(exception.Errors, e => e.PropertyName == "PokemonId" && e.AttemptedValue?.Equals(_pokemon.Id.ToGuid()) == true
      && e.ErrorCode == "UnconsciousValidator" && e.ErrorMessage == "An unconscious Pokémon cannot use moves.");
  }

  [Fact(DisplayName = "UseMove: it should throw ValidationException when the Pokémon has never learned the move.")]
  public void Given_NeverLearnedMove_When_UseMove_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => _pokemon.UseMove(_endure));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "MoveId" && e.AttemptedValue?.Equals(_endure.Id.ToGuid()) == true
      && e.ErrorCode == "LearnedMoveValidator" && e.ErrorMessage == "The Pokémon has never learned the move." && e.CustomState is not null);
  }

  [Theory(DisplayName = "UseMove: it should use a move while decreasing power points.")]
  [InlineData(1)]
  [InlineData(4)]
  public void Given_PowerPointCost_When_UseMove_Then_StaminaDecreased(byte powerPointCostValue)
  {
    PowerPoints powerPointCost = new(powerPointCostValue);
    int expectedStamina = _pokemon.Stamina;

    _pokemon.LearnMove(_armThrust, position: 0, level: null, MoveLearningMethod.Evolving);
    _pokemon.UseMove(_armThrust, powerPointCost, staminaCost: 0, _actorId);
    Assert.Equal(expectedStamina, _pokemon.Stamina);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveUsed used && used.ActorId == _actorId
      && used.MoveId == _armThrust.Id && used.PowerPointCost == powerPointCost && used.StaminaCost == 0);

    PokemonMove move = _pokemon.LearnedMoves[_armThrust.Id];
    Assert.Equal(move.MaximumPowerPoints - powerPointCostValue, move.CurrentPowerPoints);
  }

  [Theory(DisplayName = "UseMove: it should use a move while decreasing stamina.")]
  [InlineData(1)]
  [InlineData(999)]
  public void Given_StaminaCost_When_UseMove_Then_StaminaDecreased(int staminaCost)
  {
    Assert.True(staminaCost > 0);
    int expectedCost = Math.Min(staminaCost, _pokemon.Stamina);
    int expectedStamina = _pokemon.Stamina - expectedCost;

    _pokemon.LearnMove(_armThrust, position: 0, level: null, MoveLearningMethod.Evolving);
    _pokemon.UseMove(_armThrust, powerPointCost: null, staminaCost, _actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveUsed used && used.ActorId == _actorId
      && used.MoveId == _armThrust.Id && used.PowerPointCost is null && used.StaminaCost == expectedCost);

    PokemonMove move = _pokemon.LearnedMoves[_armThrust.Id];
    Assert.Equal(move.MaximumPowerPoints, move.CurrentPowerPoints);

    if (expectedStamina <= 0)
    {
      Assert.True(_pokemon.IsUnconscious);
      Assert.Equal(0, _pokemon.Stamina);
    }
    else
    {
      Assert.False(_pokemon.IsUnconscious);
      Assert.Equal(expectedStamina, _pokemon.Stamina);
    }
  }
}
