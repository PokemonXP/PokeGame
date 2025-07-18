using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonMovesTests
{
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;

  private readonly Pokemon _hedwidge;

  public PokemonMovesTests()
  {
    _hedwidge = new(
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
  }

  [Theory(DisplayName = "LearnMove: it should add the move to the list when the Pokémon has not reached the move limit.")]
  [InlineData(null)]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void Given_MoveLimitNotReached_When_LearnMove_Then_MoveAdded(int? position)
  {
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(35)); // NOTE(fpion): Tackle
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Growl
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Leafage

    MoveId moveId = MoveId.NewId(); // NOTE(fpion): Astonish
    PowerPoints powerPoints = new(15);
    ActorId actorId = ActorId.NewId();
    Assert.True(_hedwidge.LearnMove(moveId, powerPoints, position, actorId));

    Assert.Contains(_hedwidge.Changes, change => change is PokemonMoveLearned learned && learned.ActorId == actorId
      && learned.PowerPoints == powerPoints && learned.Position is null);

    PokemonMove move = new(moveId, powerPoints.Value, powerPoints.Value, powerPoints, IsMastered: false, _hedwidge.Level, TechnicalMachine: false);
    Assert.Equal(move, _hedwidge.AllMoves[move.MoveId]);
    Assert.Equal(Pokemon.MoveLimit, _hedwidge.Moves.Count);
    Assert.Equal(move, _hedwidge.Moves.Last());
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

    _hedwidge.LearnMove(tackle, new PowerPoints(35));
    _hedwidge.LearnMove(growl, new PowerPoints(40));
    _hedwidge.LearnMove(leafage, new PowerPoints(40));
    _hedwidge.LearnMove(astonish, new PowerPoints(15));

    PowerPoints powerPoints = new(40);
    ActorId actorId = ActorId.NewId();
    Assert.True(_hedwidge.LearnMove(peck, powerPoints, position, actorId));

    Assert.Contains(_hedwidge.Changes, change => change is PokemonMoveLearned learned && learned.ActorId == actorId
      && learned.PowerPoints == powerPoints && learned.Position == position);

    PokemonMove move = new(peck, powerPoints.Value, powerPoints.Value, powerPoints, IsMastered: false, _hedwidge.Level, TechnicalMachine: false);
    Assert.Equal(move, _hedwidge.AllMoves[move.MoveId]);
    Assert.Equal(Pokemon.MoveLimit, _hedwidge.Moves.Count);
    Assert.Equal(move, _hedwidge.Moves.ElementAt(position));

    Assert.Equal(5, _hedwidge.AllMoves.Count);
    Assert.True(_hedwidge.AllMoves.ContainsKey(tackle));
    Assert.True(_hedwidge.AllMoves.ContainsKey(growl));
    Assert.True(_hedwidge.AllMoves.ContainsKey(leafage));
    Assert.True(_hedwidge.AllMoves.ContainsKey(astonish));
    Assert.True(_hedwidge.AllMoves.ContainsKey(peck));
  }

  [Fact(DisplayName = "LearnMove: it should return false when the Pokémon has already learned the move.")]
  public void Given_MoveAlreadyLearned_When_LearnMove_Then_FalseReturned()
  {
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    _hedwidge.LearnMove(moveId, powerPoints);
    Assert.False(_hedwidge.LearnMove(moveId, powerPoints));
  }

  [Fact(DisplayName = "LearnMove: it should throw ArgumentNullException when the position is null and the Pokémon has reached the move limit.")]
  public void Given_MoveLimitReachedPositionNull_When_LearnMove_Then_ArgumentNullException()
  {
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(35)); // NOTE(fpion): Tackle
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Growl
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Leafage
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(15)); // NOTE(fpion): Astonish
    var exception = Assert.Throws<ArgumentNullException>(() => _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(35))); // NOTE(fpion): Peck
    Assert.Equal("position", exception.ParamName);
  }

  [Theory(DisplayName = "LearnMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_LearnMove_Then_ArgumentOutOfRangeException(int source)
  {
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _hedwidge.LearnMove(moveId, powerPoints, source));
    Assert.Equal("position", exception.ParamName);
  }

  [Fact(DisplayName = "MasterMove: it should master a move a Pokémon has learned.")]
  public void Given_MoveLearned_When_MasterMove_Then_MoveMastered()
  {
    MoveId leafage = MoveId.NewId();
    _hedwidge.LearnMove(leafage, new PowerPoints(40));

    ActorId actorId = ActorId.NewId();
    Assert.True(_hedwidge.MasterMove(leafage, actorId));
    Assert.True(_hedwidge.AllMoves[leafage].IsMastered);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonMoveMastered mastered && mastered.ActorId == actorId && mastered.MoveId == leafage);

    _hedwidge.ClearChanges();
    Assert.True(_hedwidge.MasterMove(leafage));
    Assert.False(_hedwidge.HasChanges);
  }

  [Fact(DisplayName = "MasterMove: it should return false when the Pokémon has not learned the move yet.")]
  public void Given_NotLearned_When_MasterMove_Then_FalseReturned()
  {
    Assert.False(_hedwidge.MasterMove(MoveId.NewId()));
  }

  [Fact(DisplayName = "RelearnMove: it should return false when the Pokémon has never learned the move.")]
  public void Given_NeverLearned_When_RelearnMove_Then_FalseReturned()
  {
    MoveId moveId = MoveId.NewId();
    Assert.False(_hedwidge.RelearnMove(moveId, position: 0));
  }

  [Fact(DisplayName = "RelearnMove: it should return false when the Pokémon has not forgot the move.")]
  public void Given_CurrentlyLearned_When_RelearnMove_Then_FalseReturned()
  {
    MoveId moveId = MoveId.NewId();
    _hedwidge.LearnMove(moveId, new PowerPoints(40));
    Assert.False(_hedwidge.RelearnMove(moveId, position: 0));
  }

  [Fact(DisplayName = "RelearnMove: it should relearn the specified move and return true.")]
  public void Given_ForgottenMove_When_RelearnMove_Then_MoveRelearned()
  {
    ActorId actorId = ActorId.NewId();

    MoveId tackle = MoveId.NewId();
    MoveId growl = MoveId.NewId();
    MoveId leafage = MoveId.NewId();
    MoveId astonish = MoveId.NewId();
    MoveId peck = MoveId.NewId();

    _hedwidge.LearnMove(tackle, new PowerPoints(35), actorId);
    _hedwidge.LearnMove(growl, new PowerPoints(40), actorId);
    _hedwidge.LearnMove(leafage, new PowerPoints(40), actorId);
    _hedwidge.LearnMove(astonish, new PowerPoints(15), actorId);
    _hedwidge.LearnMove(peck, new PowerPoints(35), position: 3, actorId);

    Assert.True(_hedwidge.RelearnMove(astonish, position: 1, actorId));
    Assert.Contains(_hedwidge.Changes, change => change is PokemonMoveRelearned relearned && relearned.ActorId == actorId
      && relearned.MoveId == astonish && relearned.Position == 1);

    Assert.Equal(tackle, _hedwidge.Moves.ElementAt(0).MoveId);
    Assert.Equal(astonish, _hedwidge.Moves.ElementAt(1).MoveId);
    Assert.Equal(leafage, _hedwidge.Moves.ElementAt(2).MoveId);
    Assert.Equal(peck, _hedwidge.Moves.ElementAt(3).MoveId);
  }

  [Theory(DisplayName = "RelearnMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_RelearnMove_Then_ArgumentOutOfRangeException(int position)
  {
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _hedwidge.RelearnMove(moveId, position));
    Assert.Equal("position", exception.ParamName);
  }

  [Fact(DisplayName = "SwitchMoves: it should exchange two moves position.")]
  public void Given_Moves_When_SwitchMoves_Then_PositionExchanged()
  {
    MoveId tackle = MoveId.NewId();
    MoveId growl = MoveId.NewId();
    MoveId leafage = MoveId.NewId();

    _hedwidge.LearnMove(tackle, new PowerPoints(35));
    _hedwidge.LearnMove(growl, new PowerPoints(40));
    _hedwidge.LearnMove(leafage, new PowerPoints(40));

    ActorId actorId = ActorId.NewId();
    _hedwidge.SwitchMoves(1, 2, actorId);
    Assert.Equal(tackle, _hedwidge.Moves.ElementAt(0).MoveId);
    Assert.Equal(leafage, _hedwidge.Moves.ElementAt(1).MoveId);
    Assert.Equal(growl, _hedwidge.Moves.ElementAt(2).MoveId);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonMovesSwitched switched
      && switched.ActorId == actorId && switched.Source == 1 && switched.Destination == 2);
  }

  [Fact(DisplayName = "SwitchMoves: it should not do anything when source and destination moves are the same.")]
  public void Given_SamePosition_When_SwitchMoves_Then_Nothing()
  {
    _hedwidge.ClearChanges();
    _hedwidge.SwitchMoves(1, 1);
    Assert.False(_hedwidge.HasChanges);
  }

  [Fact(DisplayName = "SwitchMoves: it should not do anything when the destination move is empty.")]
  public void Given_DestinationEmpty_When_SwitchMoves_Then_Nothing()
  {
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(35));
    _hedwidge.ClearChanges();

    _hedwidge.SwitchMoves(source: 0, destination: 3);
    Assert.False(_hedwidge.HasChanges);
  }

  [Fact(DisplayName = "SwitchMoves: it should not do anything when the source move is empty.")]
  public void Given_SourceEmpty_When_SwitchMoves_Then_Nothing()
  {
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(35));
    _hedwidge.ClearChanges();

    _hedwidge.SwitchMoves(source: 2, destination: 0);
    Assert.False(_hedwidge.HasChanges);
  }

  [Theory(DisplayName = "SwitchMoves: it should throw ArgumentOutOfRangeException when the destination is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_DestinationOutOfBounds_When_SwitchMoves_Then_ArgumentOutOfRangeException(int destination)
  {
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _hedwidge.SwitchMoves(source: 0, destination));
    Assert.Equal("destination", exception.ParamName);
  }

  [Theory(DisplayName = "SwitchMoves: it should throw ArgumentOutOfRangeException when the source is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_SourceOutOfBounds_When_SwitchMoves_Then_ArgumentOutOfRangeException(int source)
  {
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _hedwidge.SwitchMoves(source, destination: 0));
    Assert.Equal("source", exception.ParamName);
  }

  [Theory(DisplayName = "UseTechnicalMachine: it should add the move to the list when the Pokémon has not reached the move limit.")]
  [InlineData(null)]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void Given_MoveLimitNotReached_When_UseTechnicalMachine_Then_MoveAdded(int? position)
  {
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(35)); // NOTE(fpion): Tackle
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Growl
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Leafage

    MoveId moveId = MoveId.NewId(); // NOTE(fpion): Astonish
    PowerPoints powerPoints = new(15);
    ActorId actorId = ActorId.NewId();
    Assert.True(_hedwidge.UseTechnicalMachine(moveId, powerPoints, position, actorId));

    Assert.Contains(_hedwidge.Changes, change => change is PokemonTechnicalMachineUsed learned && learned.ActorId == actorId
      && learned.PowerPoints == powerPoints && learned.Position is null);

    PokemonMove move = new(moveId, powerPoints.Value, powerPoints.Value, powerPoints, IsMastered: false, _hedwidge.Level, TechnicalMachine: true);
    Assert.Equal(move, _hedwidge.AllMoves[move.MoveId]);
    Assert.Equal(Pokemon.MoveLimit, _hedwidge.Moves.Count);
    Assert.Equal(move, _hedwidge.Moves.Last());
  }

  [Theory(DisplayName = "UseTechnicalMachine: it should replace a move in the list when the Pokémon has reached the move limit.")]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void Given_MoveLimitReached_When_UseTechnicalMachine_Then_MoveReplaced(int position)
  {
    MoveId tackle = MoveId.NewId();
    MoveId growl = MoveId.NewId();
    MoveId leafage = MoveId.NewId();
    MoveId astonish = MoveId.NewId();
    MoveId peck = MoveId.NewId();

    _hedwidge.LearnMove(tackle, new PowerPoints(35));
    _hedwidge.LearnMove(growl, new PowerPoints(40));
    _hedwidge.LearnMove(leafage, new PowerPoints(40));
    _hedwidge.LearnMove(astonish, new PowerPoints(15));

    PowerPoints powerPoints = new(40);
    ActorId actorId = ActorId.NewId();
    Assert.True(_hedwidge.UseTechnicalMachine(peck, powerPoints, position, actorId));

    Assert.Contains(_hedwidge.Changes, change => change is PokemonTechnicalMachineUsed learned && learned.ActorId == actorId
      && learned.PowerPoints == powerPoints && learned.Position == position);

    PokemonMove move = new(peck, powerPoints.Value, powerPoints.Value, powerPoints, IsMastered: false, _hedwidge.Level, TechnicalMachine: true);
    Assert.Equal(move, _hedwidge.AllMoves[move.MoveId]);
    Assert.Equal(Pokemon.MoveLimit, _hedwidge.Moves.Count);
    Assert.Equal(move, _hedwidge.Moves.ElementAt(position));

    Assert.Equal(5, _hedwidge.AllMoves.Count);
    Assert.True(_hedwidge.AllMoves.ContainsKey(tackle));
    Assert.True(_hedwidge.AllMoves.ContainsKey(growl));
    Assert.True(_hedwidge.AllMoves.ContainsKey(leafage));
    Assert.True(_hedwidge.AllMoves.ContainsKey(astonish));
    Assert.True(_hedwidge.AllMoves.ContainsKey(peck));
  }

  [Fact(DisplayName = "UseTechnicalMachine: it should return false when the Pokémon has already learned the move.")]
  public void Given_MoveAlreadyLearned_When_UseTechnicalMachine_Then_FalseReturned()
  {
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    _hedwidge.LearnMove(moveId, powerPoints);
    Assert.False(_hedwidge.UseTechnicalMachine(moveId, powerPoints));
  }

  [Fact(DisplayName = "UseTechnicalMachine: it should throw ArgumentNullException when the position is null and the Pokémon has reached the move limit.")]
  public void Given_MoveLimitReachedPositionNull_When_UseTechnicalMachine_Then_ArgumentNullException()
  {
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(35)); // NOTE(fpion): Tackle
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Growl
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Leafage
    _hedwidge.LearnMove(MoveId.NewId(), new PowerPoints(15)); // NOTE(fpion): Astonish
    var exception = Assert.Throws<ArgumentNullException>(() => _hedwidge.UseTechnicalMachine(MoveId.NewId(), new PowerPoints(35))); // NOTE(fpion): Peck
    Assert.Equal("position", exception.ParamName);
  }

  [Theory(DisplayName = "UseTechnicalMachine: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_UseTechnicalMachine_Then_ArgumentOutOfRangeException(int position)
  {
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _hedwidge.UseTechnicalMachine(moveId, powerPoints, position));
    Assert.Equal("position", exception.ParamName);
  }
}
