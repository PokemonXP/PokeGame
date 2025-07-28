using FluentValidation;
using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Storage.Events;
using PokeGame.Core.Trainers;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Storage;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonStorageTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;

  private readonly Item _pokeBall;
  private readonly Location _location = new("Collège de l’Épervier");

  private readonly Trainer _trainer;
  private readonly PokemonStorage _storage;

  public PokemonStorageTests()
  {
    _species = new PokemonSpecies(new Number(499), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "pignite"), new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow, new EggCycles(20));

    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(7));

    Ability blaze = new(new UniqueName(_uniqueNameSettings, "blaze"));
    Ability thickFat = new(new UniqueName(_uniqueNameSettings, "thick-fat"));
    Sprites sprites = new(new Url("https://www.pokegame.com/assets/img/pokemon/pignite.png"), new Url("https://www.pokegame.com/assets/img/pokemon/pignite-shiny.png"));
    _form = new Form(_variety, _variety.UniqueName, new FormTypes(PokemonType.Fire, PokemonType.Fighting),
      new FormAbilities(blaze, secondary: null, thickFat), new BaseStatistics(90, 93, 55, 70, 55, 55),
      new Yield(146, 0, 2, 0, 0, 0, 0), sprites, isDefault: true, height: new Height(10), weight: new Weight(555));

    _pokemon = new(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!), experience: 7028);

    PokeBallProperties properties = new(catchMultiplier: 1.0, heal: false, baseFriendship: 0, friendshipMultiplier: 1.0);
    _pokeBall = new(new UniqueName(_uniqueNameSettings, "poke-ball"), properties, new Price(200));

    _trainer = new(new License("Q-613357-7"), new UniqueName(_uniqueNameSettings, "elliotto"));
    _storage = new PokemonStorage(_trainer);
  }

  [Fact(DisplayName = "Add: it should store a Pokémon into its trainer storage.")]
  public void Given_CorrectOwner_When_Add_Then_Stored()
  {
    _pokemon.Receive(_trainer, _pokeBall, _location);

    _storage.Add(_pokemon, _actorId);
    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId);

    PokemonSlot slot = new(new Position(0));
    Assert.Equal(slot, _storage.Slots[_pokemon.Id]);
    Assert.Equal(_pokemon.Id, _storage.Pokemon[slot]);
    Assert.Equal([_pokemon.Id], _storage.GetParty());
    Assert.Empty(_storage.GetBoxes());

    _storage.ClearChanges();
    _storage.Add(_pokemon);
    Assert.False(_storage.HasChanges);
    Assert.Empty(_storage.Changes);

    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId && moved.Slot == slot);
  }

  [Fact(DisplayName = "Add: it should throw ValidationException when the Pokémon has no owner.")]
  public void Given_NoOwner_When_Add_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => _storage.Add(_pokemon));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "PokemonId" && e.AttemptedValue.Equals(_pokemon.Id.ToGuid())
      && e.ErrorCode == "OwnershipValidator" && e.CustomState is not null
      && e.ErrorMessage == $"The Pokémon current trainer must be 'Id={_trainer.Id.ToGuid()}'.");
  }

  [Fact(DisplayName = "Add: it should throw ArgumentException when the Pokémon is owned by another trainer.")]
  public void Given_AnotherOwner_When_Add_Then_ArgumentException()
  {
    Trainer trainer = new(new License("Q-123456-3"), new UniqueName(_uniqueNameSettings, "adrianna"), TrainerGender.Female);
    _pokemon.Receive(trainer, _pokeBall, _location);

    var exception = Assert.Throws<ValidationException>(() => _storage.Add(_pokemon));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "PokemonId" && e.AttemptedValue.Equals(_pokemon.Id.ToGuid())
      && e.ErrorCode == "OwnershipValidator" && e.CustomState is not null
      && e.ErrorMessage == $"The Pokémon current trainer must be 'Id={_trainer.Id.ToGuid()}'.");
  }

  [Fact(DisplayName = "Deposit: it should deposit the Pokémon and move other party members.")]
  public void Given_PokemonParty_When_Deposit_Then_DepositedAndMoved()
  {
    Specimen other = new(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!));
    Specimen egg = new(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "egg"), _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!), eggCycles: _species.EggCycles);

    other.Receive(_trainer, _pokeBall, _location);
    _storage.Add(other);
    other.ClearChanges();

    _pokemon.Receive(_trainer, _pokeBall, _location);
    _storage.Add(_pokemon);

    egg.Receive(_trainer, _pokeBall, _location);
    _storage.Add(egg);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [_pokemon.Id] = _pokemon,
      [other.Id] = other,
      [egg.Id] = egg
    };
    _storage.Deposit(_pokemon, party, _actorId);

    PokemonSlot slot = new(new Position(0), new Box(0));
    Assert.Equal(slot, _storage.Slots[_pokemon.Id]);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId && stored.Slot == slot);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonDeposited deposited && deposited.ActorId == _actorId && deposited.Slot == slot);

    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonDeposited deposited && deposited.ActorId == _actorId && deposited.Slot == slot);
    Assert.False(other.HasChanges);
    Assert.Empty(other.Changes);
    Assert.True(egg.HasChanges);
    Assert.Contains(egg.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId && moved.Slot.Box is null && moved.Slot.Position.Value == 1);

    _storage.ClearChanges();
    _pokemon.ClearChanges();
    other.ClearChanges();
    egg.ClearChanges();

    _storage.Deposit(_pokemon, party, _actorId);
    Assert.False(_storage.HasChanges);
    Assert.Empty(_storage.Changes);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
    Assert.False(other.HasChanges);
    Assert.Empty(other.Changes);
    Assert.False(egg.HasChanges);
    Assert.Empty(egg.Changes);
  }

  [Fact(DisplayName = "Deposit: it should throw ValidationException when the party would be empty.")]
  public void Given_EmptyParty_When_Deposit_Then_ValidationException()
  {
    _pokemon.Receive(_trainer, _pokeBall, _location);
    _storage.Add(_pokemon);

    Specimen egg = new(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "egg"),
      _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(),
      _randomizer.PokemonGender(_variety.GenderRatio!), eggCycles: _species.EggCycles);
    egg.Receive(_trainer, _pokeBall, _location);
    _storage.Add(egg);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [egg.Id] = egg
    };
    var exception = Assert.Throws<ValidationException>(() => _storage.Deposit(_pokemon, party, _actorId));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "PokemonId" && e.AttemptedValue.Equals(_pokemon.Id.ToGuid())
      && e.ErrorCode == "NonEmptyPartyValidator"
      && e.ErrorMessage == "The trainer Pokémon party must contain at least another non-egg Pokémon.");
  }

  [Fact(DisplayName = "Deposit: it should throw ValidationException when the Pokémon has no owner.")]
  public void Given_NoOwner_When_Deposit_Then_ValidationException()
  {
    Dictionary<PokemonId, Specimen> party = [];
    var exception = Assert.Throws<ValidationException>(() => _storage.Deposit(_pokemon, party));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "PokemonId" && e.AttemptedValue.Equals(_pokemon.Id.ToGuid())
      && e.ErrorCode == "OwnershipValidator" && e.CustomState is not null
      && e.ErrorMessage == $"The Pokémon current trainer must be 'Id={_trainer.Id.ToGuid()}'.");
  }

  [Fact(DisplayName = "Deposit: it should throw ValidationException when the Pokémon is owned by another trainer.")]
  public void Given_AnotherOwner_When_Deposit_Then_ValidationException()
  {
    Trainer trainer = new(new License("Q-123456-3"), new UniqueName(_uniqueNameSettings, "adrianna"), TrainerGender.Female);
    _pokemon.Receive(trainer, _pokeBall, _location);

    Dictionary<PokemonId, Specimen> party = [];
    var exception = Assert.Throws<ValidationException>(() => _storage.Deposit(_pokemon, party));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "PokemonId" && e.AttemptedValue.Equals(_pokemon.Id.ToGuid())
      && e.ErrorCode == "OwnershipValidator" && e.CustomState is not null
      && e.ErrorMessage == $"The Pokémon current trainer must be 'Id={_trainer.Id.ToGuid()}'.");
  }
}
