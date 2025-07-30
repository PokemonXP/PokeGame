using FluentValidation;
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

namespace PokeGame.Core.Battles;

[Trait(Traits.Category, Categories.Unit)]
public class BattleTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly UniqueNameSettings _uniqueNameSetings = new();

  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;
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

    _trainer = new Trainer(new License("Q-123456-3"), new UniqueName(_uniqueNameSetings, "elliotto"));
  }

  [Fact(DisplayName = "Start: it should start the battle.")]
  public void Given_NotStarted_When_Start_Then_Started()
  {
    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");
    Battle battle = Battle.WildPokemon(name, location, [_trainer], [_pokemon]);

    battle.Start(_actorId);
    Assert.Equal(BattleStatus.Started, battle.Status);
    Assert.True(battle.HasChanges);
    Assert.Contains(battle.Changes, change => change is BattleStarted started && started.ActorId == _actorId);

    battle.ClearChanges();
    battle.Start();
    Assert.False(battle.HasChanges);
    Assert.Empty(battle.Changes);
  }

  [Fact(DisplayName = "Trainer: it should create a trainer battle.")]
  public void Given_Valid_When_Trainer_Then_Battle()
  {
    Trainer opponent = new(new License("Q-123456-3"), new UniqueName(_uniqueNameSetings, "regina"), TrainerGender.Female);

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
    Assert.Equal(opponent.Id, battle.TrainerOpponents.Single());
    Assert.Empty(battle.PokemonOpponents);
  }

  [Fact(DisplayName = "Trainer: it should throw ArgumentException when champions are empty.")]
  public void Given_EmptyChampions_When_Trainer_Then_ArgumentException()
  {
    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");
    var exception = Assert.Throws<ArgumentException>(() => Battle.Trainer(name, location, champions: [], opponents: []));
    Assert.Equal("champions", exception.ParamName);
    Assert.StartsWith("At least one champion trainer must be provided.", exception.Message);
  }

  [Fact(DisplayName = "Trainer: it should throw ArgumentException when opponents are empty.")]
  public void Given_EmptyOpponents_When_Trainer_Then_ArgumentException()
  {
    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");
    var exception = Assert.Throws<ArgumentException>(() => Battle.Trainer(name, location, champions: [_trainer], opponents: []));
    Assert.Equal("opponents", exception.ParamName);
    Assert.StartsWith("At least one opponent trainer must be provided.", exception.Message);
  }

  [Fact(DisplayName = "Trainer: it should throw ValidationException when a trainer appears on both sides of the battle.")]
  public void Given_TrainerBothSides_When_Trainer_Then_ValidationException()
  {
    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");
    var exception = Assert.Throws<ValidationException>(() => Battle.Trainer(name, location, [_trainer], [_trainer]));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "TrainerId" && e.AttemptedValue?.Equals(_trainer.Id.ToGuid()) == true
      && e.ErrorCode == "TrainerBattleValidator" && e.ErrorMessage == "The trainer cannot appear on both sides of the battle.");
  }

  [Fact(DisplayName = "WildPokemon: it should create a wild Pokémon battle.")]
  public void Given_Valid_When_WildPokemon_Then_Battle()
  {
    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");
    Url url = new("https://bulbapedia.bulbagarden.net/wiki/Pok%C3%A9mon_battle");
    Notes notes = new("This is my first battle.");
    BattleId battleId = BattleId.NewId();
    Battle battle = Battle.WildPokemon(name, location, [_trainer], [_pokemon], url, notes, _actorId, battleId);

    Assert.True(battle.HasChanges);
    Assert.Contains(battle.Changes, change => change is WildPokemonBattleCreated created && created.ActorId == _actorId);

    Assert.Equal(battleId, battle.Id);
    Assert.Equal(BattleKind.WildPokemon, battle.Kind);
    Assert.Equal(BattleStatus.Created, battle.Status);
    Assert.Equal(name, battle.Name);
    Assert.Equal(location, battle.Location);
    Assert.Equal(url, battle.Url);
    Assert.Equal(notes, battle.Notes);
    Assert.Equal(_trainer.Id, battle.Champions.Single());
    Assert.Empty(battle.TrainerOpponents);
    Assert.Equal(_pokemon.Id, battle.PokemonOpponents.Single());
  }

  [Fact(DisplayName = "WildPokemon: it should throw ArgumentException when champions are empty.")]
  public void Given_EmptyChampions_When_WildPokemon_Then_ArgumentException()
  {
    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");
    var exception = Assert.Throws<ArgumentException>(() => Battle.WildPokemon(name, location, champions: [], opponents: []));
    Assert.Equal("champions", exception.ParamName);
    Assert.StartsWith("At least one champion trainer must be provided.", exception.Message);
  }

  [Fact(DisplayName = "WildPokemon: it should throw ArgumentException when opponents are empty.")]
  public void Given_EmptyOpponents_When_WildPokemon_Then_ArgumentException()
  {
    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");
    var exception = Assert.Throws<ArgumentException>(() => Battle.WildPokemon(name, location, champions: [_trainer], opponents: []));
    Assert.Equal("opponents", exception.ParamName);
    Assert.StartsWith("At least one opponent Pokémon must be provided.", exception.Message);
  }

  [Fact(DisplayName = "WildPokemon: it should throw ValidationException when a Pokémon has an owner.")]
  public void Given_NotWildPokemon_When_WildPokemon_Then_ValidationException()
  {
    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");

    Item pokeBall = new(new UniqueName(_uniqueNameSetings, "poke-ball"), new PokeBallProperties(), new Price(200));
    _pokemon.Catch(_trainer, pokeBall, location);

    var exception = Assert.Throws<ValidationException>(() => Battle.WildPokemon(name, location, [_trainer], [_pokemon]));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "Opponents" && e.AttemptedValue?.Equals(_pokemon.Id.ToGuid()) == true
      && e.ErrorCode == "WildPokemonValidator" && e.ErrorMessage == "The Pokémon must be a wild Pokémon.");
  }

  [Fact(DisplayName = "WildPokemon: it should throw ValidationException when a Pokémon is still an egg.")]
  public void Given_EggPokemon_When_WildPokemon_Then_ValidationException()
  {
    Specimen pokemon = new(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(), _randomizer.PokemonNature(),
      _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!), eggCycles: _species.EggCycles);

    DisplayName name = new("My First Battle");
    Location location = new("Collège de l’Épervier");
    var exception = Assert.Throws<ValidationException>(() => Battle.WildPokemon(name, location, [_trainer], [pokemon]));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "Opponents" && e.AttemptedValue?.Equals(pokemon.Id.ToGuid()) == true
      && e.ErrorCode == "EggValidator" && e.ErrorMessage == "The Pokémon must not be an egg.");
  }
}
