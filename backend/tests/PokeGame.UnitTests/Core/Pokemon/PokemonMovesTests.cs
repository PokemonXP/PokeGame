﻿using Krakenar.Core;
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
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;

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
  }

  [Fact(DisplayName = "LearnMove: it should learn a move by evolving.")]
  public void Given_NoLevel_When_LearnMove_Then_EvolvingMove()
  {
    ActorId actorId = ActorId.NewId();

    Move tackle = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new Accuracy(100), new Power(40), new PowerPoints(35));
    Move tailWhip = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new Accuracy(100), power: null, new PowerPoints(30));
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    Move endure = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "endure"), accuracy: null, power: null, new PowerPoints(10));
    _pokemon.LearnMove(tackle, position: 0, new Level(1));
    _pokemon.LearnMove(tailWhip, position: 1, new Level(1));
    _pokemon.LearnMove(ember, position: 2, new Level(1));
    _pokemon.LearnMove(endure, position: 3, new Level(1));

    Move armThrust = new(PokemonType.Fighting, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "arm-thrust"), new Accuracy(100), new Power(15), new PowerPoints(20));
    int position = 3;
    MoveLearningMethod method = MoveLearningMethod.Evolving;
    Notes notes = new("Learned by evolving from Tepig to Pignite.");
    Assert.True(_pokemon.LearnMove(armThrust, position, level: null, method, notes, actorId));
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveLearned learned && learned.ActorId == actorId && learned.MoveId == armThrust.Id && learned.Position == position);

    KeyValuePair<MoveId, PokemonMove> move = _pokemon.CurrentMoves.ElementAt(position);
    Assert.Equal(armThrust.Id, move.Key);
    Assert.Equal(armThrust.PowerPoints.Value, move.Value.CurrentPowerPoints);
    Assert.Equal(armThrust.PowerPoints.Value, move.Value.MaximumPowerPoints);
    Assert.Equal(armThrust.PowerPoints, move.Value.ReferencePowerPoints);
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
    Move tackle = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new Accuracy(100), new Power(40), new PowerPoints(35));
    Move tailWhip = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new Accuracy(100), power: null, new PowerPoints(30));
    _pokemon.LearnMove(tackle, position: 0, new Level(1));
    _pokemon.LearnMove(tailWhip, position: 1, new Level(1));

    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    Assert.True(_pokemon.LearnMove(ember, position, new Level(1)));
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveLearned learned && learned.MoveId == ember.Id && learned.Position == 2);

    KeyValuePair<MoveId, PokemonMove> move = _pokemon.CurrentMoves.ElementAt(2);
    Assert.Equal(ember.Id, move.Key);
    Assert.Equal(ember.PowerPoints.Value, move.Value.CurrentPowerPoints);
    Assert.Equal(ember.PowerPoints.Value, move.Value.MaximumPowerPoints);
    Assert.Equal(ember.PowerPoints, move.Value.ReferencePowerPoints);
    Assert.False(move.Value.IsMastered);
    Assert.Equal(1, move.Value.Level.Value);
    Assert.Equal(MoveLearningMethod.LevelingUp, move.Value.Method);
    Assert.Null(move.Value.ItemId);
    Assert.Null(move.Value.Notes);
  }

  [Fact(DisplayName = "LearnMove: it should return false when the Pokémon has already learned the move.")]
  public void Given_MoveAlreadyLearned_When_LearnMove_Then_FalseReturned()
  {
    Move move = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new Accuracy(100), new Power(40), new PowerPoints(35));
    Assert.True(_pokemon.LearnMove(move));
    Assert.False(_pokemon.LearnMove(move));
  }

  [Theory(DisplayName = "LearnMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_LearnMove_Then_ArgumentOutOfRangeException(int position)
  {
    Move move = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new Accuracy(100), new Power(40), new PowerPoints(35));
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.LearnMove(move, position));
    Assert.Equal("position", exception.ParamName);
  }

  [Fact(DisplayName = "RememberMove: it should remember a forgotten move and return true.")]
  public void Given_ForgottenMove_When_RememberMove_Then_MoveRemembered()
  {
    ActorId actorId = ActorId.NewId();

    Move tackle = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new Accuracy(100), new Power(40), new PowerPoints(35));
    Move tailWhip = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new Accuracy(100), power: null, new PowerPoints(30));
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    Move endure = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "endure"), accuracy: null, power: null, new PowerPoints(10));
    Move defenseCurl = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "defense-curl"), accuracy: null, power: null, new PowerPoints(40));
    _pokemon.LearnMove(tackle);
    _pokemon.LearnMove(tailWhip);
    _pokemon.LearnMove(ember);
    _pokemon.LearnMove(endure);
    _pokemon.LearnMove(defenseCurl);

    int position = 1;
    Assert.True(_pokemon.RememberMove(defenseCurl, position, actorId));
    KeyValuePair<MoveId, PokemonMove> move = _pokemon.CurrentMoves.ElementAt(position);
    Assert.Equal(defenseCurl.Id, move.Key);

    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveRemembered remembered && remembered.MoveId == defenseCurl.Id && remembered.Position == position);
  }

  [Fact(DisplayName = "RememberMove: it should return true when the move is currently learned.")]
  public void Given_AlreadyLearned_When_RememberMove_Then_FalseReturned()
  {
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    _pokemon.LearnMove(ember, position: null, new Level(6));
    _pokemon.ClearChanges();

    Assert.True(_pokemon.RememberMove(ember, position: 0));
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "RememberMove: it should return false when the Pokémon has not learned the move.")]
  public void Given_MoveNeverLearned_When_RememberMove_Then_FalseReturned()
  {
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    Assert.False(_pokemon.RememberMove(ember, position: 0));
  }

  [Theory(DisplayName = "RememberMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_RememberMove_Then_ArgumentOutOfRangeException(int position)
  {
    Move tackle = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new Accuracy(100), new Power(40), new PowerPoints(35));
    Move tailWhip = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new Accuracy(100), power: null, new PowerPoints(30));
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    Move endure = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "endure"), accuracy: null, power: null, new PowerPoints(10));
    Move defenseCurl = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "defense-curl"), accuracy: null, power: null, new PowerPoints(40));
    _pokemon.LearnMove(tackle);
    _pokemon.LearnMove(tailWhip);
    _pokemon.LearnMove(ember);
    _pokemon.LearnMove(endure);
    _pokemon.LearnMove(defenseCurl);

    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.RememberMove(defenseCurl, position));
    Assert.Equal("position", exception.ParamName);
  }

  [Fact(DisplayName = "SwitchMoves: it should exchange two moves.")]
  public void Given_SourceAndDestinationMoves_When_SwitchMoves_Then_MovesExchanged()
  {
    ActorId actorId = ActorId.NewId();

    Move tackle = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new Accuracy(100), new Power(40), new PowerPoints(35));
    Move tailWhip = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new Accuracy(100), power: null, new PowerPoints(30));
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));

    _pokemon.LearnMove(tackle);
    _pokemon.LearnMove(tailWhip);
    _pokemon.LearnMove(ember);

    int source = 0;
    int destination = 2;
    _pokemon.SwitchMoves(source, destination, actorId);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveSwitched switched && switched.ActorId == actorId
      && switched.Source == source && switched.Destination == destination);

    Assert.Equal(ember.Id, _pokemon.CurrentMoves.ElementAt(0).Key);
    Assert.Equal(tailWhip.Id, _pokemon.CurrentMoves.ElementAt(1).Key);
    Assert.Equal(tackle.Id, _pokemon.CurrentMoves.ElementAt(2).Key);
  }

  [Fact(DisplayName = "SwitchMoves: it should not do anything when the destination move is empty.")]
  public void Given_SourceEqualDestination_When_SwitchMoves_Then_NoChange()
  {
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    _pokemon.LearnMove(ember);
    _pokemon.ClearChanges();

    _pokemon.SwitchMoves(source: 0, destination: 1);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "SwitchMoves: it should not do anything when the source and destination moves are the same.")]
  public void Given_SourceNotCurrent_When_SwitchMoves_Then_NoChange()
  {
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    _pokemon.LearnMove(ember);
    _pokemon.ClearChanges();

    _pokemon.SwitchMoves(source: 0, destination: 0);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "SwitchMoves: it should not do anything when the source move is empty.")]
  public void Given_DestinationNotCurrent_When_SwitchMoves_Then_NoChange()
  {
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new Accuracy(100), new Power(40), new PowerPoints(25));
    _pokemon.LearnMove(ember);
    _pokemon.ClearChanges();

    _pokemon.SwitchMoves(source: 1, destination: 0);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Theory(DisplayName = "SwitchMoves: it should throw ArgumentOutOfRangeException when the destination is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_DestinationOutOfBounds_When_SwitchMoves_Then_ArgumentOutOfRangeException(int destination)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.SwitchMoves(source: 0, destination));
    Assert.Equal("destination", exception.ParamName);
  }

  [Theory(DisplayName = "SwitchMoves: it should throw ArgumentOutOfRangeException when the source is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_SourceOutOfBounds_When_SwitchMoves_Then_ArgumentOutOfRangeException(int source)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.SwitchMoves(source, destination: 0));
    Assert.Equal("source", exception.ParamName);
  }
}
