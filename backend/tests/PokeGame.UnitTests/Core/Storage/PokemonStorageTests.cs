using Bogus;
using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Regions;
using PokeGame.Core.Storage.Events;
using PokeGame.Core.Trainers;
using ValidationException = FluentValidation.ValidationException;

namespace PokeGame.Core.Storage;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonStorageTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly Faker _faker = new();

  private readonly Location _college = new("Collège de l’Épervier");
  private readonly Location _park = new("Parc Boidelys");
  private readonly Item _pokeBall;
  private readonly Trainer _trainer;
  private readonly PokemonStorage _storage;

  public PokemonStorageTests()
  {
    UniqueNameSettings uniqueNameSettings = new();

    _pokeBall = new Item(new UniqueName(uniqueNameSettings, "poke-ball"), new PokeBallProperties(), new Price(200));
    _trainer = _faker.Trainer();
    _storage = new PokemonStorage(_trainer);
  }

  [Fact(DisplayName = "Deposit: it should deposit the Pokémon.")]
  public void Given_InParty_When_Deposit_Then_Deposited()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    rowlet.ClearChanges();
    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly
    };
    _storage.Deposit(cutiefly, party, _actorId);

    Assert.False(rowlet.HasChanges);
    Assert.Empty(rowlet.Changes);

    Assert.NotNull(cutiefly.Slot?.Box);
    Assert.Equal(0, cutiefly.Slot.Position.Value);
    Assert.Equal(0, cutiefly.Slot.Box.Value);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonDeposited deposited && deposited.ActorId == _actorId);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == cutiefly.Id && stored.Slot == cutiefly.Slot);
  }

  [Fact(DisplayName = "Deposit: it should move the Pokémon after the deposited Pokémon in the previous party slot.")]
  public void Given_InPartyAfter_When_Deposit_Then_MovedToPrevious()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    rowlet.ClearChanges();
    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(rowlet, party, _actorId);

    Assert.NotNull(rowlet.Slot?.Box);
    Assert.Equal(0, rowlet.Slot.Position.Value);
    Assert.Equal(0, rowlet.Slot.Box.Value);
    Assert.True(rowlet.HasChanges);
    Assert.Single(rowlet.Changes);
    Assert.Contains(rowlet.Changes, change => change is PokemonDeposited deposited && deposited.ActorId == _actorId);

    Assert.NotNull(cutiefly.Slot);
    Assert.Equal(0, cutiefly.Slot.Position.Value);
    Assert.Null(cutiefly.Slot.Box);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId && moved.Slot == cutiefly.Slot);

    Assert.NotNull(eevee.Slot);
    Assert.Equal(1, eevee.Slot.Position.Value);
    Assert.Null(eevee.Slot.Box);
    Assert.True(eevee.HasChanges);
    Assert.Contains(eevee.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId && moved.Slot == eevee.Slot);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == rowlet.Id && stored.Slot == rowlet.Slot);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == cutiefly.Id && stored.Slot == cutiefly.Slot);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == eevee.Id && stored.Slot == eevee.Slot);
  }

  [Fact(DisplayName = "Deposit: it should throw ArgumentException when the Pokémon is already in a box.")]
  public void Given_InBox_When_Deposit_Then_ArgumentException()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Receive(_trainer, _pokeBall, _college);
    _storage.Store(cutiefly);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly
    };
    _storage.Deposit(rowlet, party);

    var exception = Assert.Throws<ArgumentException>(() => _storage.Deposit(rowlet, party));
    Assert.Equal("pokemon", exception.ParamName);
    Assert.StartsWith($"The Pokémon '{rowlet}' is not in trainer's 'Id={_trainer.Id}' party.", exception.Message);
  }

  [Fact(DisplayName = "Deposit: it should throw ArgumentException when the Pokémon is not in storage.")]
  public void Given_NotStored_When_Deposit_Then_ArgumentException()
  {
    Specimen pokemon = new PokemonBuilder(_faker).Build();
    var exception = Assert.Throws<ArgumentException>(() => _storage.Deposit(pokemon, new Dictionary<PokemonId, Specimen>()));
    Assert.Equal("pokemon", exception.ParamName);
    Assert.StartsWith($"The Pokémon '{pokemon}' was not found in trainer's 'Id={_trainer.Id}' storage.", exception.Message);
  }

  [Fact(DisplayName = "Deposit: it should throw ValidationException when the operation would leave the party empty.")]
  public void Given_EmptyPartyOtherwise_When_Deposit_Then_ValidationException()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(cutiefly, party);

    var exception = Assert.Throws<ValidationException>(() => _storage.Deposit(rowlet, party));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "TrainerId" && e.AttemptedValue?.Equals(_trainer.Id.ToGuid()) == true
      && e.ErrorCode == "NotEmptyPartyValidator" && e.ErrorMessage == "The operation would leave the trainer party empty of non-egg Pokémon."
      && e.CustomState is not null);
  }

  [Fact(DisplayName = "GetParty: it should return the correct party member IDs.")]
  public void Given_PartyMembers_When_GetParty_Then_CorrectIds()
  {
    Assert.Empty(_storage.GetParty());

    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(cutiefly, party);

    Assert.Equal([rowlet.Id, eevee.Id], _storage.GetParty());
  }

  [Fact(DisplayName = "Move: it should move the Pokémon after the moved Pokémon in the previous party slot.")]
  public void Given_InPartyAfter_When_Move_Then_MovedToPrevious()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    rowlet.ClearChanges();
    PokemonSlot slot = new(new Position(1), new Box(1));
    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Move(cutiefly, slot, party, _actorId);

    Assert.False(rowlet.HasChanges);
    Assert.Empty(rowlet.Changes);
    Assert.Equal(new PokemonSlot(new Position(0)), rowlet.Slot);

    Assert.Equal(slot, cutiefly.Slot);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId);

    Assert.Equal(new PokemonSlot(new Position(1)), eevee.Slot);
    Assert.True(eevee.HasChanges);
    Assert.Contains(eevee.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == eevee.Id && stored.Slot == eevee.Slot);
  }

  [Fact(DisplayName = "Move: it should move the Pokémon in the specified slot.")]
  public void Given_ValidOperation_When_Move_Then_Moved()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);
    rowlet.ClearChanges();

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    PokemonSlot slot = new(new Position(1), new Box(1));
    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [eevee.Id] = eevee
    };
    _storage.Move(eevee, slot, party, _actorId);

    Assert.Equal(slot, eevee.Slot);
    Assert.True(eevee.HasChanges);
    Assert.Contains(eevee.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == eevee.Id && stored.Slot == eevee.Slot);

    Assert.False(rowlet.HasChanges);
    Assert.Empty(rowlet.Changes);
    Assert.Equal(new PokemonSlot(new Position(0)), rowlet.Slot);
  }

  [Fact(DisplayName = "Move: it should not do anything when the Pokémon already occupies the slot.")]
  public void Given_AlreadyInSlot_When_Move_Then_DoNothing()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [eevee.Id] = eevee
    };
    _storage.Deposit(eevee, party);

    Assert.NotNull(eevee.Slot);
    _storage.ClearChanges();
    _storage.Move(eevee, eevee.Slot, party);
    Assert.False(_storage.HasChanges);
    Assert.Empty(_storage.Changes);
  }

  [Fact(DisplayName = "Move: it should not move any other Pokémon if it is already in a box.")]
  public void Given_InBox_When_Move_Then_OnlyPokemonMoved()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    rowlet.ClearChanges();
    cutiefly.ClearChanges();

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(eevee, party);

    PokemonSlot slot = new(new Position(1), new Box(1));
    _storage.Move(eevee, slot, party, _actorId);

    Assert.Equal(new PokemonSlot(new Position(0)), rowlet.Slot);
    Assert.False(rowlet.HasChanges);
    Assert.Empty(rowlet.Changes);

    Assert.Equal(new PokemonSlot(new Position(1)), cutiefly.Slot);
    Assert.False(cutiefly.HasChanges);
    Assert.Empty(cutiefly.Changes);

    Assert.Equal(slot, eevee.Slot);
    Assert.True(eevee.HasChanges);
    Assert.Contains(eevee.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == eevee.Id && stored.Slot == slot);
  }

  [Fact(DisplayName = "Move: it should throw ArgumentException when the Pokémon is not in storage.")]
  public void Given_NotStored_When_Move_Then_ArgumentException()
  {
    Specimen pokemon = new PokemonBuilder(_faker).Build();
    PokemonSlot slot = new(new Position(0));
    Dictionary<PokemonId, Specimen> party = [];
    var exception = Assert.Throws<ArgumentException>(() => _storage.Move(pokemon, slot, party));
    Assert.Equal("pokemon", exception.ParamName);
    Assert.StartsWith($"The Pokémon '{pokemon}' was not found in trainer's 'Id={_trainer.Id}' storage.", exception.Message);
  }

  [Fact(DisplayName = "Move: it should throw ArgumentException when the slot is in the party.")]
  public void Given_SlotInParty_When_Move_Then_ArgumentException()
  {
    Specimen pokemon = new PokemonBuilder(_faker).Build();
    pokemon.Receive(_trainer, _pokeBall, _college);
    _storage.Store(pokemon);

    PokemonSlot slot = new(new Position(PokemonSlot.PartySize - 1));
    Dictionary<PokemonId, Specimen> party = new()
    {
      [pokemon.Id] = pokemon
    };
    var exception = Assert.Throws<ArgumentException>(() => _storage.Move(pokemon, slot, party));
  }

  [Fact(DisplayName = "Move: it should throw ValidationException when the operation would leave the party empty.")]
  public void Given_EmptyPartyOtherwise_When_Move_Then_ValidationException()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(cutiefly, party);

    PokemonSlot slot = new(new Position(1), new Box(1));
    var exception = Assert.Throws<ValidationException>(() => _storage.Move(rowlet, slot, party));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "TrainerId" && e.AttemptedValue?.Equals(_trainer.Id.ToGuid()) == true
      && e.ErrorCode == "NotEmptyPartyValidator" && e.ErrorMessage == "The operation would leave the trainer party empty of non-egg Pokémon."
      && e.CustomState is not null);
  }

  [Fact(DisplayName = "Move: it should throw ValidationException when the slot is already occupied.")]
  public void Given_SlotConflict_When_Move_Then_ValidationException()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Catch(_trainer, _pokeBall, _park);
    _storage.Store(eevee);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(cutiefly, party);

    Assert.NotNull(cutiefly.Slot);
    var exception = Assert.Throws<ValidationException>(() => _storage.Move(eevee, cutiefly.Slot, party));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "TrainerId" && e.AttemptedValue?.Equals(_trainer.Id.ToGuid()) == true
      && e.ErrorCode == "PokemonSlotNotEmpty" && e.ErrorMessage == "The specified Pokémon slot is not empty."
      && e.CustomState is not null);
  }

  [Fact(DisplayName = "Release: it should move the Pokémon after the released Pokémon in the previous party slot.")]
  public void Given_InPartyAfter_When_Release_Then_MovedToPrevious()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(eevee, party, _actorId);
    _storage.Swap(rowlet, cutiefly, party);

    _storage.Release(cutiefly, party, _actorId);

    Assert.Equal(new PokemonSlot(new Position(0)), rowlet.Slot);
    Assert.True(rowlet.HasChanges);
    Assert.Contains(rowlet.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId);

    Assert.Null(cutiefly.Ownership);
    Assert.Null(cutiefly.Slot);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonReleased released && released.ActorId == _actorId);

    Assert.Equal(new PokemonSlot(new Position(0), new Box(0)), eevee.Slot);
    Assert.True(eevee.HasChanges);
    Assert.Contains(eevee.Changes, change => change is PokemonDeposited deposited && deposited.ActorId == _actorId && deposited.Slot == eevee.Slot);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == rowlet.Id && stored.Slot == rowlet.Slot);
    Assert.Contains(_storage.Changes, change => change is PokemonRemoved removed && removed.PokemonId == cutiefly.Id);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == eevee.Id && stored.Slot == eevee.Slot);
  }

  [Fact(DisplayName = "Release: it should release a Pokémon in a box.")]
  public void Given_InBox_When_Release_Then_Released()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    rowlet.ClearChanges();
    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(cutiefly, party);

    _storage.Release(cutiefly, party, _actorId);

    Assert.False(rowlet.HasChanges);
    Assert.Empty(rowlet.Changes);

    Assert.Null(cutiefly.Ownership);
    Assert.Null(cutiefly.Slot);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonReleased released && released.ActorId == _actorId);

    Assert.Equal(new PokemonSlot(new Position(1)), eevee.Slot);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonRemoved removed && removed.PokemonId == cutiefly.Id);
  }

  [Fact(DisplayName = "Release: it should release a Pokémon in the party.")]
  public void Given_InParty_When_Release_Then_Released()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    rowlet.ClearChanges();
    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Swap(cutiefly, eevee, party);

    _storage.Release(cutiefly, party, _actorId);

    Assert.False(rowlet.HasChanges);
    Assert.Empty(rowlet.Changes);

    Assert.Null(cutiefly.Ownership);
    Assert.Null(cutiefly.Slot);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonReleased released && released.ActorId == _actorId);

    Assert.Equal(new PokemonSlot(new Position(1)), eevee.Slot);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonRemoved removed && removed.PokemonId == cutiefly.Id);
  }

  [Fact(DisplayName = "Release: it should throw ArgumentException when the Pokémon is not in storage.")]
  public void Given_NotStored_When_Release_Then_ArgumentException()
  {
    Specimen pokemon = new PokemonBuilder(_faker).Build();
    Dictionary<PokemonId, Specimen> party = [];
    var exception = Assert.Throws<ArgumentException>(() => _storage.Release(pokemon, party));
    Assert.Equal("pokemon", exception.ParamName);
    Assert.StartsWith($"The Pokémon '{pokemon}' was not found in trainer's 'Id={_trainer.Id}' storage.", exception.Message);
  }

  [Fact(DisplayName = "Release: it should throw ValidationException when the operation would leave the party empty.")]
  public void Given_EmptyPartyOtherwise_When_Release_Then_ValidationException()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(rowlet, party);

    var exception = Assert.Throws<ValidationException>(() => _storage.Release(cutiefly, party));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "TrainerId" && e.AttemptedValue?.Equals(_trainer.Id.ToGuid()) == true
      && e.ErrorCode == "NotEmptyPartyValidator" && e.ErrorMessage == "The operation would leave the trainer party empty of non-egg Pokémon."
      && e.CustomState is not null);
  }

  [Fact(DisplayName = "Remove: it should remove the Pokémon from storage.")]
  public void Given_Pokemon_When_Remove_Then_Removed()
  {
    Specimen pokemon = new PokemonBuilder(_faker).Build();
    pokemon.Receive(_trainer, _pokeBall, _college);
    _storage.Store(pokemon);
    _storage.ClearChanges();

    Assert.True(_storage.Remove(pokemon, _actorId));
    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonRemoved removed && removed.ActorId == _actorId && removed.PokemonId == pokemon.Id);
  }

  [Fact(DisplayName = "Store: it should store the Pokémon.")]
  public void Given_ValidOwner_When_Store_Then_Stored()
  {
    Specimen pokemon = new PokemonBuilder(_faker).Build();
    pokemon.Receive(_trainer, _pokeBall, _college);

    _storage.Store(pokemon, _actorId);
    Assert.NotNull(pokemon.Slot);
    Assert.Equal(0, pokemon.Slot.Position.Value);
    Assert.Null(pokemon.Slot.Box);
    Assert.True(pokemon.HasChanges);
    Assert.Contains(pokemon.Changes, change => change is PokemonMoved moved && moved.ActorId == _actorId);
    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored
      && stored.ActorId == _actorId && stored.PokemonId == pokemon.Id && stored.Slot == pokemon.Slot);

    pokemon.ClearChanges();
    _storage.ClearChanges();
    _storage.Store(pokemon);
    Assert.False(pokemon.HasChanges);
    Assert.Empty(pokemon.Changes);
    Assert.False(_storage.HasChanges);
    Assert.Empty(_storage.Changes);
  }

  [Fact(DisplayName = "Store: it should throw ArgumentException when the Pokémon has no owner.")]
  public void Given_NoOwner_When_Store_Then_ArgumentException()
  {
    Specimen pokemon = new PokemonBuilder(_faker).Build();
    var exception = Assert.Throws<ArgumentException>(() => _storage.Store(pokemon));
    Assert.Equal("pokemon", exception.ParamName);
    Assert.StartsWith($"The Pokémon owner trainer 'Id=<null>' must be '{_trainer.Id}'.", exception.Message);
  }

  [Fact(DisplayName = "Store: it should throw ArgumentException when the Pokémon owner is not valid.")]
  public void Given_InvalidOwner_When_Store_Then_ArgumentException()
  {
    Specimen pokemon = new PokemonBuilder(_faker).Build();

    Trainer trainer = _faker.Trainer();
    pokemon.Receive(trainer, _pokeBall, _college);

    var exception = Assert.Throws<ArgumentException>(() => _storage.Store(pokemon));
    Assert.Equal("pokemon", exception.ParamName);
    Assert.StartsWith($"The Pokémon owner trainer 'Id={trainer.Id}' must be '{_trainer.Id}'.", exception.Message);
  }

  [Fact(DisplayName = "Swap: it should swap Pokémon in boxes.")]
  public void Given_BothInBoxes_When_Swap_Then_Swapped()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);
    Assert.NotNull(cutiefly.Slot);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);
    Assert.NotNull(eevee.Slot);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(cutiefly, party);
    _storage.Deposit(eevee, party);
    PokemonSlot sourceSlot = cutiefly.Slot;
    PokemonSlot destinationSlot = eevee.Slot;

    _storage.Swap(cutiefly, eevee, party, _actorId);
    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is StoredPokemonSwapped swapped && swapped.ActorId == _actorId
      && swapped.SourceId == cutiefly.Id && swapped.DestinationId == eevee.Id);

    Assert.Equal(destinationSlot, cutiefly.Slot);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonSwapped swapped && swapped.ActorId == _actorId);

    Assert.Equal(sourceSlot, eevee.Slot);
    Assert.True(eevee.HasChanges);
    Assert.Contains(eevee.Changes, change => change is PokemonSwapped swapped && swapped.ActorId == _actorId);
  }

  [Fact(DisplayName = "Swap: it should swap Pokémon in the party.")]
  public void Given_BothInParty_When_Swap_Then_Swapped()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);
    Assert.NotNull(rowlet.Slot);
    PokemonSlot sourceSlot = rowlet.Slot;

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);
    Assert.NotNull(cutiefly.Slot);
    PokemonSlot destinationSlot = cutiefly.Slot;

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly
    };
    _storage.Swap(rowlet, cutiefly, party, _actorId);
    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is StoredPokemonSwapped swapped && swapped.ActorId == _actorId
      && swapped.SourceId == rowlet.Id && swapped.DestinationId == cutiefly.Id);

    Assert.Equal(destinationSlot, rowlet.Slot);
    Assert.True(rowlet.HasChanges);
    Assert.Contains(rowlet.Changes, change => change is PokemonSwapped swapped && swapped.ActorId == _actorId);

    Assert.Equal(sourceSlot, cutiefly.Slot);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonSwapped swapped && swapped.ActorId == _actorId);
  }

  [Fact(DisplayName = "Swap: it should swap Pokémon mixed in the party and a box.")]
  public void Given_InPartyAndABox_When_Swap_Then_Swapped()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);
    Assert.NotNull(rowlet.Slot);
    PokemonSlot sourceSlot = rowlet.Slot;

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);
    Assert.NotNull(cutiefly.Slot);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly
    };
    _storage.Deposit(cutiefly, party);
    PokemonSlot destinationSlot = cutiefly.Slot;

    _storage.Swap(rowlet, cutiefly, party, _actorId);
    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is StoredPokemonSwapped swapped && swapped.ActorId == _actorId
      && swapped.SourceId == rowlet.Id && swapped.DestinationId == cutiefly.Id);

    Assert.Equal(destinationSlot, rowlet.Slot);
    Assert.True(rowlet.HasChanges);
    Assert.Contains(rowlet.Changes, change => change is PokemonSwapped swapped && swapped.ActorId == _actorId);

    Assert.Equal(sourceSlot, cutiefly.Slot);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonSwapped swapped && swapped.ActorId == _actorId);
  }

  [Fact(DisplayName = "Swap: it should throw ArgumentException when the destination Pokémon is not in storage.")]
  public void Given_DestinationNotStored_When_Swap_Then_ArgumentException()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly
    };

    var exception = Assert.Throws<ArgumentException>(() => _storage.Swap(rowlet, cutiefly, party));
    Assert.Equal("destination", exception.ParamName);
    Assert.StartsWith($"The Pokémon '{cutiefly}' was not found in trainer's 'Id={_trainer.Id}' storage.", exception.Message);
  }

  [Fact(DisplayName = "Swap: it should throw ArgumentException when the source Pokémon is not in storage.")]
  public void Given_SourceNotStored_When_Swap_Then_ArgumentException()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly
    };

    var exception = Assert.Throws<ArgumentException>(() => _storage.Swap(rowlet, cutiefly, party));
    Assert.Equal("source", exception.ParamName);
    Assert.StartsWith($"The Pokémon '{rowlet}' was not found in trainer's 'Id={_trainer.Id}' storage.", exception.Message);
  }

  [Fact(DisplayName = "Swap: it should throw ValidationException when the operation would leave the party empty.")]
  public void Given_EmptyPartyOtherwise_When_Swap_Then_ValidationException()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Catch(_trainer, _pokeBall, _park);
    _storage.Store(cutiefly);

    Specimen eevee = new PokemonBuilder(_faker).WithSpecies("eevee").IsEgg().Build();
    eevee.Receive(_trainer, _pokeBall, _college);
    _storage.Store(eevee);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly,
      [eevee.Id] = eevee
    };
    _storage.Deposit(cutiefly, party);
    _storage.Deposit(eevee, party);

    var exception = Assert.Throws<ValidationException>(() => _storage.Swap(rowlet, eevee, party));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "TrainerId" && e.AttemptedValue?.Equals(_trainer.Id.ToGuid()) == true
      && e.ErrorCode == "NotEmptyPartyValidator" && e.ErrorMessage == "The operation would leave the trainer party empty of non-egg Pokémon."
      && e.CustomState is not null);
  }

  [Fact(DisplayName = "Withdraw: it should throw ArgumentException when the Pokémon is already in the party.")]
  public void Given_InParty_When_Withdraw_Then_ArgumentException()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Receive(_trainer, _pokeBall, _college);
    _storage.Store(cutiefly);

    var exception = Assert.Throws<ArgumentException>(() => _storage.Withdraw(rowlet));
    Assert.Equal("pokemon", exception.ParamName);
    Assert.StartsWith($"The Pokémon '{rowlet}' is already in trainer's 'Id={_trainer.Id}' party.", exception.Message);
  }

  [Fact(DisplayName = "Withdraw: it should throw ArgumentException when the Pokémon is not in storage.")]
  public void Given_NotStored_When_Withdraw_Then_ArgumentException()
  {
    Specimen pokemon = new PokemonBuilder(_faker).Build();
    var exception = Assert.Throws<ArgumentException>(() => _storage.Withdraw(pokemon));
    Assert.Equal("pokemon", exception.ParamName);
    Assert.StartsWith($"The Pokémon '{pokemon}' was not found in trainer's 'Id={_trainer.Id}' storage.", exception.Message);
  }

  [Fact(DisplayName = "Withdraw: it should throw ValidationException when the trainer's party is full.")]
  public void Given_PartyIsFull_When_Withdraw_Then_ValidationException()
  {
    List<Specimen> pokemonList = new(capacity: PokemonSlot.PartySize + 1);
    for (int i = 0; i < pokemonList.Capacity; i++)
    {
      Specimen pokemon = new PokemonBuilder(_faker).Build();
      pokemonList.Add(pokemon);

      pokemon.Receive(_trainer, _pokeBall, _faker.PickRandom(_college, _park));
      _storage.Store(pokemon);
    }

    var exception = Assert.Throws<ValidationException>(() => _storage.Withdraw(pokemonList.Last()));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "TrainerId" && e.AttemptedValue?.Equals(_trainer.Id.ToGuid()) == true
      && e.ErrorCode == "TrainerPartyIsFull" && e.ErrorMessage == "The specified trainer party is full."
      && e.CustomState is not null);
  }

  [Fact(DisplayName = "Withdraw: it should withdraw the Pokémon.")]
  public void Given_InBox_When_Withdraw_Then_Withdrew()
  {
    Specimen rowlet = new PokemonBuilder(_faker).WithSpecies("rowlet").Build();
    rowlet.Receive(_trainer, _pokeBall, _college);
    _storage.Store(rowlet);

    Specimen cutiefly = new PokemonBuilder(_faker).WithSpecies("cutiefly").Build();
    cutiefly.Receive(_trainer, _pokeBall, _college);
    _storage.Store(cutiefly);

    Dictionary<PokemonId, Specimen> party = new()
    {
      [rowlet.Id] = rowlet,
      [cutiefly.Id] = cutiefly
    };
    _storage.Deposit(cutiefly, party);

    _storage.Withdraw(cutiefly, _actorId);

    Assert.NotNull(cutiefly.Slot);
    Assert.Equal(1, cutiefly.Slot.Position.Value);
    Assert.Null(cutiefly.Slot.Box);
    Assert.True(cutiefly.HasChanges);
    Assert.Contains(cutiefly.Changes, change => change is PokemonWithdrew withdrew && withdrew.ActorId == _actorId);

    Assert.True(_storage.HasChanges);
    Assert.Contains(_storage.Changes, change => change is PokemonStored stored && stored.ActorId == _actorId
      && stored.PokemonId == cutiefly.Id && stored.Slot == cutiefly.Slot);
  }
}
