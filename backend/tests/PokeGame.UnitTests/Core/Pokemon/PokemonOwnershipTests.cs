using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonOwnershipTests
{
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;

  private readonly Trainer _trainer;
  private readonly Item _pokeBall;
  private readonly Location _location = new("Collège de l’Épervier");

  public PokemonOwnershipTests()
  {
    _species = new PokemonSpecies(new Number(499), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "pignite"), new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow);

    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(7));

    Ability blaze = new(new UniqueName(_uniqueNameSettings, "blaze"));
    Ability thickFat = new(new UniqueName(_uniqueNameSettings, "thick-fat"));
    Sprites sprites = new(new Url("https://www.pokegame.com/assets/img/pokemon/pignite.png"), new Url("https://www.pokegame.com/assets/img/pokemon/pignite-shiny.png"));
    _form = new Form(_variety, _variety.UniqueName, new FormTypes(PokemonType.Fire, PokemonType.Fighting),
      new FormAbilities(blaze, secondary: null, thickFat), new BaseStatistics(90, 93, 55, 70, 55, 55),
      new Yield(146, 0, 2, 0, 0, 0, 0), sprites, isDefault: true, height: new Height(10), weight: new Weight(555));

    _pokemon = new(
      _species,
      _variety,
      _form,
      _species.UniqueName,
      _randomizer.PokemonSize(),
      _randomizer.PokemonNature(),
      _randomizer.IndividualValues(),
      _randomizer.PokemonGender(_variety.GenderRatio!),
      experience: 7028);

    PokeBallProperties properties = new(catchMultiplier: 1.0, heal: false, baseFriendship: 0, friendshipMultiplier: 1.0);
    _pokeBall = new(new UniqueName(_uniqueNameSettings, "poke-ball"), properties, new Price(200));

    _trainer = new(new License("Q-613357-7"), new UniqueName(_uniqueNameSettings, "elliotto"));
  }

  [Fact(DisplayName = "Receive: a gifted Pokémon should retain its original trainer and Poké Ball.")]
  public void Given_Gifted_When_Receive_Then_OriginalTrainerAndPokeBallRetained()
  {
    _pokemon.Receive(_trainer, _pokeBall, _location);
    Assert.Equal(_trainer.Id, _pokemon.OriginalTrainerId);
    Assert.Equal(_trainer.Id, _pokemon.Ownership?.TrainerId);

    Trainer trainer = new(new License("Q-123456-3"), new UniqueName(_uniqueNameSettings, "regina"), TrainerGender.Female);

    PokeBallProperties properties = new(catchMultiplier: 1.5, heal: false, baseFriendship: 0, friendshipMultiplier: 1.0);
    Item greatBall = new(new UniqueName(_uniqueNameSettings, "great-ball"), properties, new Price(600));
    _pokemon.Receive(trainer, greatBall, _location);
    Assert.Equal(_trainer.Id, _pokemon.OriginalTrainerId);
    Assert.NotNull(_pokemon.Ownership);
    Assert.Equal(trainer.Id, _pokemon.Ownership.TrainerId);
    Assert.Equal(_pokeBall.Id, _pokemon.Ownership.PokeBallId);
  }

  [Fact(DisplayName = "Receive: a Pokémon should be received correctly.")]
  public void Given_Arguments_When_Receive_Then_Received()
  {
    ActorId actorId = ActorId.NewId();

    Level level = new(5);
    DateTime metOn = new DateTime(2000, 1, 1);
    Description description = new("Received at Lv.5, at Collège de l’Épervier, on January 1st, 2000.");
    PokemonSlot slot = new(new Position(2), new Box(1));
    _pokemon.Receive(_trainer, _pokeBall, _location, level, metOn, description, slot, actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonReceived received && received.ActorId == actorId);

    Assert.Equal(_trainer.Id, _pokemon.OriginalTrainerId);

    Ownership? ownership = _pokemon.Ownership;
    Assert.NotNull(ownership);
    Assert.Equal(OwnershipKind.Received, ownership.Kind);
    Assert.Equal(_trainer.Id, ownership.TrainerId);
    Assert.Equal(_pokeBall.Id, ownership.PokeBallId);
    Assert.Equal(level, ownership.Level);
    Assert.Equal(_location, ownership.Location);
    Assert.Equal(metOn, ownership.MetOn);
    Assert.Equal(description, ownership.Description);

    Assert.Equal(slot, _pokemon.Slot);
  }

  [Fact(DisplayName = "Receive: a Pokémon should be received using default values.")]
  public void Given_DefaultsArguments_When_Receive_Then_DefaultValues()
  {
    _pokemon.Receive(_trainer, _pokeBall, _location);
    DateTime metOn = ((PokemonReceived)_pokemon.Changes.Single(change => change is PokemonReceived)).OccurredOn;

    Ownership? ownership = _pokemon.Ownership;
    Assert.NotNull(ownership);
    Assert.Equal(_pokemon.Level, ownership.Level.Value);
    Assert.Equal(metOn, ownership.MetOn);
    Assert.Null(ownership.Description);

    Assert.NotNull(_pokemon.Slot);
    Assert.Equal(0, _pokemon.Slot.Position.Value);
    Assert.Null(_pokemon.Slot.Box);
  }

  [Fact(DisplayName = "Receive: it should throw ArgumentException when the Poké Ball category is not valid.")]
  public void Given_InvalidPokeBallCategory_When_Receive_Then_ArgumentException()
  {
    MedicineProperties properties = new(isHerbal: false, healing: 20, isHealingPercentage: false, revives: false,
      statusCondition: null, allConditions: false, powerPoints: 0, isPowerPointPercentage: false, restoreAllMoves: false);
    Item potion = new(new UniqueName(_uniqueNameSettings, "potion"), properties);
    var exception = Assert.Throws<ArgumentException>(() => _pokemon.Receive(_trainer, potion, _location));
    Assert.Equal("pokeBall", exception.ParamName);
    Assert.StartsWith("The item category should be 'PokeBall'.", exception.Message);
  }

  [Fact(DisplayName = "Receive: it should throw ArgumentOutOfRangeException when the level exceeds the Pokémon level.")]
  public void Given_LevelExceedingCurrent_When_Receive_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.Receive(_trainer, _pokeBall, _location, new Level(100)));
    Assert.Equal("level", exception.ParamName);
  }

  [Fact(DisplayName = "Receive: it should throw ArgumentOutOfRangeException when the Pokémon will be met in the future.")]
  public void Given_MetInTheFuture_When_Receive_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _pokemon.Receive(_trainer, _pokeBall, _location, metOn: DateTime.Now.AddDays(1)));
    Assert.Equal("metOn", exception.ParamName);
  }

  [Fact(DisplayName = "Release: it should release a Pokémon in a box.")]
  public void Given_PokemonInBox_When_Release_Then_Released()
  {
    ActorId actorId = ActorId.NewId();

    PokemonSlot slot = new(new Position(0), new Box(0));
    _pokemon.Receive(_trainer, _pokeBall, _location, slot: slot);

    _pokemon.Release(actorId);
    Assert.Null(_pokemon.OriginalTrainerId);
    Assert.Null(_pokemon.Ownership);
    Assert.Null(_pokemon.Slot);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonReleased released && released.ActorId == actorId);

    _pokemon.ClearChanges();
    _pokemon.Release();
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "Release: it should throw CannotReleasePartyPokemonException when the Pokémon is in the party.")]
  public void Given_PokemonInParty_When_Release_Then_CannotReleasePartyPokemonException()
  {
    PokemonSlot slot = new(new Position(0), Box: null);
    _pokemon.Receive(_trainer, _pokeBall, _location, slot: slot);

    var exception = Assert.Throws<CannotReleasePartyPokemonException>(() => _pokemon.Release());
    Assert.Equal(_pokemon.Id.ToGuid(), exception.PokemonId);
  }
}
