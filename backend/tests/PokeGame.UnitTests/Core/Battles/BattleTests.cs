using Bogus;
using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Battles.Events;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;
using PokeGame.Core.Varieties;
using ValidationException = FluentValidation.ValidationException;

namespace PokeGame.Core.Battles;

[Trait(Traits.Category, Categories.Unit)]
public class BattleTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly Faker _faker = new();
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly UniqueNameSettings _uniqueNameSetings = new();

  private readonly DisplayName _name = new("My First Battle");
  private readonly Location _location = new("Collège de l’Épervier");

  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;
  private readonly Item _pokeBall;
  private readonly Trainer _trainer;

  public BattleTests()
  {
    _species = new PokemonSpecies(new Number(819), PokemonCategory.Standard, new UniqueName(_uniqueNameSetings, "skwovet"), eggCycles: new EggCycles(20));
    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(4));

    Ability cheekPouch = new(new UniqueName(_uniqueNameSetings, "cheek-pouch"));
    Ability gluttony = new(new UniqueName(_uniqueNameSetings, "gluttony"));
    _form = new Form(_variety, _variety.UniqueName, new FormTypes(PokemonType.Normal), new FormAbilities(cheekPouch, gluttony),
      new BaseStatistics(70, 55, 55, 35, 35, 25), new Yield(55, 1, 0, 0, 0, 0, 0),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/skwovet.png"), new Url("https://www.pokegame.com/assets/img/pokemon/skwovet-shiny.png")),
      isDefault: true, height: new Height(3), weight: new Weight(25));

    _pokemon = new Specimen(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!));

    _pokeBall = new Item(new UniqueName(_uniqueNameSetings, "poke-ball"), new PokeBallProperties());

    _trainer = _faker.Trainer();
  }

  [Fact(DisplayName = "Start: it should start a trainer battle.")]
  public void Given_TrainerNotStarted_When_Start_Then_Started()
  {
    Trainer opponent = _faker.Trainer();
    _pokemon.Receive(opponent, _pokeBall, _location);

    Battle battle = Battle.Trainer(_name, _location, [_trainer], [opponent]);

    Specimen fuckOff = new(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!));
    fuckOff.Catch(_trainer, _pokeBall, _location);

    battle.Start([fuckOff, _pokemon], _actorId);
    Assert.Equal(BattleStatus.Started, battle.Status);

    Assert.Equal(2, battle.Pokemon.Count);
    Assert.True(battle.Pokemon[_pokemon.Id].IsActive);
    Assert.True(battle.Pokemon[fuckOff.Id].IsActive);

    Assert.True(battle.HasChanges);
    Assert.Contains(battle.Changes, change => change is BattleStarted started && started.ActorId == _actorId);
  }

  [Fact(DisplayName = "Start: it should start a wild Pokémon battle.")]
  public void Given_WildNotStarted_When_Start_Then_Started()
  {
    Battle battle = Battle.WildPokemon(_name, _location, [_trainer], [_pokemon]);

    Specimen fuckOff = new(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!));
    fuckOff.Catch(_trainer, _pokeBall, _location);

    Specimen other = new(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!));
    other.Catch(_trainer, _pokeBall, _location, slot: new PokemonSlot(new Position(1)));

    battle.Start([other, fuckOff], _actorId);
    Assert.Equal(BattleStatus.Started, battle.Status);

    Assert.Equal(3, battle.Pokemon.Count);
    Assert.True(battle.Pokemon[_pokemon.Id].IsActive);
    Assert.True(battle.Pokemon[fuckOff.Id].IsActive);
    Assert.False(battle.Pokemon[other.Id].IsActive);

    Assert.True(battle.HasChanges);
    Assert.Contains(battle.Changes, change => change is BattleStarted started && started.ActorId == _actorId);
  }

  [Fact(DisplayName = "Start: it should throw ValidationException when the battle has already started.")]
  public void Given_AlreadyStarted_When_Start_Then_ValidationException()
  {
    Battle battle = Battle.WildPokemon(_name, _location, [_trainer], [_pokemon]);

    Specimen fuckOff = new(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!));
    fuckOff.Catch(_trainer, _pokeBall, _location);

    battle.Start([fuckOff]);

    var exception = Assert.Throws<ValidationException>(() => battle.Start([fuckOff]));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "BattleId" && e.AttemptedValue?.Equals(battle.Id.ToGuid()) == true
      && e.ErrorCode == "StatusValidator" && e.ErrorMessage == "The battle must not have started.");
  }

  [Fact(DisplayName = "Trainer: it should create a trainer battle.")]
  public void Given_Valid_When_Trainer_Then_Battle()
  {
    Trainer opponent = _faker.Trainer();

    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");
    BattleId battleId = BattleId.NewId();
    Battle battle = Battle.Trainer(name, location, [_trainer], [opponent], url: null, notes: null, _actorId, battleId);

    Assert.True(battle.HasChanges);
    Assert.Contains(battle.Changes, change => change is TrainerBattleCreated created && created.ActorId == _actorId);

    Assert.Equal(battleId, battle.Id);
    Assert.Equal(BattleKind.Trainer, battle.Kind);
    Assert.Equal(BattleStatus.Created, battle.Status);
    Assert.Equal(name, battle.Name);
    Assert.Equal(location, battle.Location);
    Assert.Null(battle.Url);
    Assert.Null(battle.Notes);
    Assert.Equal(_trainer.Id, battle.Champions.Single());
    Assert.Equal(opponent.Id, battle.Opponents.Single());
    Assert.Empty(battle.Pokemon);
  }

  [Fact(DisplayName = "Trainer: it should throw ArgumentException when champions are empty.")]
  public void Given_EmptyChampions_When_Trainer_Then_ArgumentException()
  {
    var exception = Assert.Throws<ArgumentException>(() => Battle.Trainer(_name, _location, champions: [], opponents: []));
    Assert.Equal("champions", exception.ParamName);
    Assert.StartsWith("At least one champion trainer must be provided.", exception.Message);
  }

  [Fact(DisplayName = "Trainer: it should throw ArgumentException when opponents are empty.")]
  public void Given_EmptyOpponents_When_Trainer_Then_ArgumentException()
  {
    var exception = Assert.Throws<ArgumentException>(() => Battle.Trainer(_name, _location, champions: [_trainer], opponents: []));
    Assert.Equal("opponents", exception.ParamName);
    Assert.StartsWith("At least one opponent trainer must be provided.", exception.Message);
  }

  [Fact(DisplayName = "Trainer: it should throw ValidationException when a trainer appears on both sides of the battle.")]
  public void Given_TrainerBothSides_When_Trainer_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => Battle.Trainer(_name, _location, [_trainer], [_trainer]));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "TrainerId" && e.AttemptedValue?.Equals(_trainer.Id.ToGuid()) == true
      && e.ErrorCode == "TrainerBattleValidator" && e.ErrorMessage == "The trainer cannot appear on both sides of the battle.");
  }

  [Fact(DisplayName = "WildPokemon: it should create a wild Pokémon battle.")]
  public void Given_Valid_When_WildPokemon_Then_Battle()
  {
    Url url = new("https://bulbapedia.bulbagarden.net/wiki/Pok%C3%A9mon_battle");
    Notes notes = new("This is my first battle.");
    BattleId battleId = BattleId.NewId();
    Battle battle = Battle.WildPokemon(_name, _location, [_trainer], [_pokemon], url, notes, _actorId, battleId);

    Assert.True(battle.HasChanges);
    Assert.Contains(battle.Changes, change => change is WildPokemonBattleCreated created && created.ActorId == _actorId);

    Assert.Equal(battleId, battle.Id);
    Assert.Equal(BattleKind.WildPokemon, battle.Kind);
    Assert.Equal(BattleStatus.Created, battle.Status);
    Assert.Equal(_name, battle.Name);
    Assert.Equal(_location, battle.Location);
    Assert.Equal(url, battle.Url);
    Assert.Equal(notes, battle.Notes);
    Assert.Equal(_trainer.Id, battle.Champions.Single());
    Assert.Empty(battle.Opponents);
    Assert.Single(battle.Pokemon);
    Assert.True(battle.Pokemon[_pokemon.Id].IsActive);
  }

  [Fact(DisplayName = "WildPokemon: it should throw ArgumentException when champions are empty.")]
  public void Given_EmptyChampions_When_WildPokemon_Then_ArgumentException()
  {
    var exception = Assert.Throws<ArgumentException>(() => Battle.WildPokemon(_name, _location, champions: [], opponents: []));
    Assert.Equal("champions", exception.ParamName);
    Assert.StartsWith("At least one champion trainer must be provided.", exception.Message);
  }

  [Fact(DisplayName = "WildPokemon: it should throw ArgumentException when opponents are empty.")]
  public void Given_EmptyOpponents_When_WildPokemon_Then_ArgumentException()
  {
    var exception = Assert.Throws<ArgumentException>(() => Battle.WildPokemon(_name, _location, champions: [_trainer], opponents: []));
    Assert.Equal("opponents", exception.ParamName);
    Assert.StartsWith("At least one opponent Pokémon must be provided.", exception.Message);
  }

  [Fact(DisplayName = "WildPokemon: it should throw ValidationException when a Pokémon has an owner.")]
  public void Given_NotWildPokemon_When_WildPokemon_Then_ValidationException()
  {
    _pokemon.Catch(_trainer, _pokeBall, _location);

    var exception = Assert.Throws<ValidationException>(() => Battle.WildPokemon(_name, _location, [_trainer], [_pokemon]));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "Opponents" && e.AttemptedValue?.Equals(_pokemon.Id.ToGuid()) == true
      && e.ErrorCode == "WildPokemonValidator" && e.ErrorMessage == "The Pokémon must be a wild Pokémon.");
  }

  [Fact(DisplayName = "WildPokemon: it should throw ValidationException when a Pokémon is still an egg.")]
  public void Given_EggPokemon_When_WildPokemon_Then_ValidationException()
  {
    Specimen pokemon = new(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(), _randomizer.PokemonNature(),
      _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!), eggCycles: _species.EggCycles);

    var exception = Assert.Throws<ValidationException>(() => Battle.WildPokemon(_name, _location, [_trainer], [pokemon]));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "Opponents" && e.AttemptedValue?.Equals(pokemon.Id.ToGuid()) == true
      && e.ErrorCode == "EggValidator" && e.ErrorMessage == "The Pokémon must not be an egg.");
  }

  // TASK: [POKEGAME-264](https://logitar.atlassian.net/browse/POKEGAME-264)
}
