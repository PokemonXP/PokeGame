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

    Move tackle = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new PowerPoints(35), new Accuracy(100), new Power(40));
    Move tailWhip = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new PowerPoints(30), new Accuracy(100));
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new PowerPoints(25), new Accuracy(100), new Power(40));
    Move endure = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "endure"), new PowerPoints(10));
    _pokemon.LearnMove(tackle, position: 0, new Level(1));
    _pokemon.LearnMove(tailWhip, position: 1, new Level(1));
    _pokemon.LearnMove(ember, position: 2, new Level(1));
    _pokemon.LearnMove(endure, position: 3, new Level(1));

    Move armThrust = new(PokemonType.Fighting, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "arm-thrust"), new PowerPoints(20), new Accuracy(100), new Power(15));
    int position = 3;
    Notes notes = new("Learned by evolving from Tepig to Pignite.");
    Assert.True(_pokemon.LearnMove(armThrust, position, level: null, notes, actorId));
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveLearned learned && learned.ActorId == actorId && learned.MoveId == armThrust.Id && learned.Position == position);

    KeyValuePair<MoveId, PokemonMove> move = _pokemon.CurrentMoves.ElementAt(position);
    Assert.Equal(armThrust.Id, move.Key);
    Assert.Equal(armThrust.PowerPoints.Value, move.Value.CurrentPowerPoints);
    Assert.Equal(armThrust.PowerPoints.Value, move.Value.MaximumPowerPoints);
    Assert.Equal(armThrust.PowerPoints, move.Value.ReferencePowerPoints);
    Assert.False(move.Value.IsMastered);
    Assert.Equal(_pokemon.Level, move.Value.Level.Value);
    Assert.Equal(MoveLearningMethod.Evolving, move.Value.Method);
    Assert.Null(move.Value.ItemId);
    Assert.Equal(notes, move.Value.Notes);
  }

  [Theory(DisplayName = "LearnMove: it should override the position when the move limit has not been reached.")]
  [InlineData(null)]
  [InlineData(3)]
  public void Given_LimitNotReached_When_LearnMove_Then_PositionOverriden(int? position)
  {
    Move tackle = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new PowerPoints(35), new Accuracy(100), new Power(40));
    Move tailWhip = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new PowerPoints(30), new Accuracy(100));
    _pokemon.LearnMove(tackle, position: 0, new Level(1));
    _pokemon.LearnMove(tailWhip, position: 1, new Level(1));

    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new PowerPoints(25), new Accuracy(100), new Power(40));
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
    Move move = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new PowerPoints(35), new Accuracy(100), new Power(40));
    Assert.True(_pokemon.LearnMove(move));
    Assert.False(_pokemon.LearnMove(move));
  }

  [Theory(DisplayName = "LearnMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_LearnMove_Then_ArgumentOutOfRangeException(int position)
  {
    Move move = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new PowerPoints(35), new Accuracy(100), new Power(40));
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.LearnMove(move, position));
    Assert.Equal("position", exception.ParamName);
  }

  [Fact(DisplayName = "RelearnMove: it should relearn a forgotten move and return true.")]
  public void Given_ForgottenMove_When_RelearnMove_Then_MoveRelearned()
  {
    ActorId actorId = ActorId.NewId();

    Move tackle = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new PowerPoints(35), new Accuracy(100), new Power(40));
    Move tailWhip = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new PowerPoints(30), new Accuracy(100));
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new PowerPoints(25), new Accuracy(100), new Power(40));
    Move endure = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "endure"), new PowerPoints(10));
    Move defenseCurl = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "defense-curl"), new PowerPoints(40));
    _pokemon.LearnMove(tackle);
    _pokemon.LearnMove(tailWhip);
    _pokemon.LearnMove(ember);
    _pokemon.LearnMove(endure);
    _pokemon.LearnMove(defenseCurl);

    int position = 1;
    Assert.True(_pokemon.RelearnMove(defenseCurl, position, actorId));
    KeyValuePair<MoveId, PokemonMove> move = _pokemon.CurrentMoves.ElementAt(position);
    Assert.Equal(defenseCurl.Id, move.Key);

    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoveRelearned relearned && relearned.MoveId == defenseCurl.Id && relearned.Position == position);
  }

  [Fact(DisplayName = "RelearnMove: it should return false when the move is currently learned.")]
  public void Given_AlreadyLearned_When_RelearnMove_Then_FalseReturned()
  {
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new PowerPoints(25), new Accuracy(100), new Power(40));
    _pokemon.LearnMove(ember, position: null, new Level(6));
    Assert.False(_pokemon.RelearnMove(ember, position: 0));
  }

  [Fact(DisplayName = "RelearnMove: it should return false when the Pokémon has not learned the move.")]
  public void Given_MoveNeverLearned_When_RelearnMove_Then_FalseReturned()
  {
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new PowerPoints(25), new Accuracy(100), new Power(40));
    Assert.False(_pokemon.RelearnMove(ember, position: 0));
  }

  [Theory(DisplayName = "RelearnMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_RelearnMove_Then_ArgumentOutOfRangeException(int position)
  {
    Move tackle = new(PokemonType.Normal, MoveCategory.Physical, new UniqueName(_uniqueNameSettings, "tackle"), new PowerPoints(35), new Accuracy(100), new Power(40));
    Move tailWhip = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "tail-whip"), new PowerPoints(30), new Accuracy(100));
    Move ember = new(PokemonType.Fire, MoveCategory.Special, new UniqueName(_uniqueNameSettings, "ember"), new PowerPoints(25), new Accuracy(100), new Power(40));
    Move endure = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "endure"), new PowerPoints(10));
    Move defenseCurl = new(PokemonType.Normal, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "defense-curl"), new PowerPoints(40));
    _pokemon.LearnMove(tackle);
    _pokemon.LearnMove(tailWhip);
    _pokemon.LearnMove(ember);
    _pokemon.LearnMove(endure);
    _pokemon.LearnMove(defenseCurl);

    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.RelearnMove(defenseCurl, position));
    Assert.Equal("position", exception.ParamName);
  }
}
