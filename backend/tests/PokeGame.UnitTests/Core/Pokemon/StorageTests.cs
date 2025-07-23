namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class StorageTests
{
  [Fact(DisplayName = "GetFirstEmptySlot: it should return box, 0 when the box is empty.")]
  public void Given_EmptyBox_When_GetFirstEmptySlot_Then_Party0()
  {
    List<KeyValuePair<PokemonId, PokemonSlot>> slots = new(capacity: Storage.PartySize + Storage.BoxSize);
    for (int position = 0; position < Storage.PartySize; position++)
    {
      slots.Add(new(PokemonId.NewId(), new PokemonSlot(new Position(position), Box: null)));
    }
    for (int position = 0; position < Storage.BoxSize; position++)
    {
      slots.Add(new(PokemonId.NewId(), new PokemonSlot(new Position(position), new Box(0))));
    }
    Storage storage = new(slots);

    PokemonSlot slot = storage.GetFirstEmptySlot();
    Assert.Equal(0, slot.Position.Value);
    Assert.Equal(1, slot.Box?.Value);
  }

  [Fact(DisplayName = "GetFirstEmptySlot: it should return party, 0 when storage is empty.")]
  public void Given_EmptyStorage_When_GetFirstEmptySlot_Then_Party0()
  {
    Storage storage = new(slots: []);
    PokemonSlot slot = storage.GetFirstEmptySlot();
    Assert.Equal(0, slot.Position.Value);
    Assert.Null(slot.Box);
  }

  [Fact(DisplayName = "GetFirstEmptySlot: it should return the first box available slot.")]
  public void Given_BoxNotFull_When_GetFirstEmptySlot_Then_BoxFirstAvailable()
  {
    List<KeyValuePair<PokemonId, PokemonSlot>> slots = new(capacity: 2 * Storage.PartySize);
    for (int position = 0; position < Storage.PartySize; position++)
    {
      slots.Add(new(PokemonId.NewId(), new PokemonSlot(new Position(position), Box: null)));
      slots.Add(new(PokemonId.NewId(), new PokemonSlot(new Position(position), new Box(0))));
    }
    Storage storage = new(slots);

    PokemonSlot slot = storage.GetFirstEmptySlot();
    Assert.Equal(Storage.PartySize, slot.Position.Value);
    Assert.Equal(0, slot.Box?.Value);
  }

  [Fact(DisplayName = "GetFirstEmptySlot: it should return the first party available slot.")]
  public void Given_PartyNotFull_When_GetFirstEmptySlot_Then_PartyFirstAvailable()
  {
    KeyValuePair<PokemonId, PokemonSlot>[] slots =
    [
      new(PokemonId.NewId(), new PokemonSlot(new Position(0), Box: null)),
      new(PokemonId.NewId(), new PokemonSlot(new Position(1), Box: null)),
      new(PokemonId.NewId(), new PokemonSlot(new Position(2), new Box(2)))
    ];
    Storage storage = new(slots);

    PokemonSlot slot = storage.GetFirstEmptySlot();
    Assert.Equal(2, slot.Position.Value);
    Assert.Null(slot.Box);
  }

  [Fact(DisplayName = "IsEmpty: it should return false when the slot is not empty.")]
  public void Given_NotEmpty_When_IsEmpty_Then_False()
  {
    PokemonSlot slot = new(new Position(0), new Box(0));
    Storage storage = new(
    [
      new(PokemonId.NewId(), new PokemonSlot(new Position(0), Box: null)),
      new(PokemonId.NewId(), slot)
    ]);

    Assert.False(storage.IsEmpty(slot));
  }

  [Fact(DisplayName = "IsEmpty: it should return true when the slot is empty.")]
  public void Given_Empty_When_IsEmpty_Then_True()
  {
    Storage storage = new([new(PokemonId.NewId(), new PokemonSlot(new Position(0), Box: null))]);

    PokemonSlot slot = new(new Position(0), new Box(0));
    Assert.True(storage.IsEmpty(slot));
  }

  [Fact(DisplayName = "It should create a storage with boxes.")]
  public void Given_PartyAndBoxes_When_ctor_Then_Created()
  {
    KeyValuePair<PokemonId, PokemonSlot> party = new(PokemonId.NewId(), new PokemonSlot(new Position(0), Box: null));
    KeyValuePair<PokemonId, PokemonSlot> box = new(PokemonId.NewId(), new PokemonSlot(new Position(15), new Box(3)));
    Storage storage = new([party, box]);

    Assert.Equal(2, storage.Pokemon.Count);
    Assert.Equal(party.Value, storage.Pokemon[party.Key]);
    Assert.Equal(box.Value, storage.Pokemon[box.Key]);

    Assert.Equal(2, storage.Slots.Count);
    Assert.Equal(party.Key, storage.Slots[party.Value]);
    Assert.Equal(box.Key, storage.Slots[box.Value]);

    Assert.Equal(party.Key, storage.Party.Single());

    Assert.Single(storage.Boxes);
    Assert.NotNull(box.Value.Box);
    IReadOnlyDictionary<Position, PokemonId> inBox = storage.Boxes[new Box(box.Value.Box.Value)];
    Assert.Single(inBox);
    Assert.Equal(box.Key, inBox[box.Value.Position]);
  }

  [Fact(DisplayName = "It should create a storage with empty boxes.")]
  public void Given_PartyEmptyBoxes_When_ctor_Then_Created()
  {
    KeyValuePair<PokemonId, PokemonSlot> slot = new(PokemonId.NewId(), new PokemonSlot(new Position(0), Box: null));
    Storage storage = new([slot]);

    Assert.Single(storage.Pokemon);
    Assert.Equal(slot.Value, storage.Pokemon[slot.Key]);

    Assert.Single(storage.Slots);
    Assert.Equal(slot.Key, storage.Slots[slot.Value]);

    Assert.Equal(slot.Key, storage.Party.Single());

    Assert.Empty(storage.Boxes);
  }

  [Fact(DisplayName = "It should create an empty storage.")]
  public void Given_NoSlot_When_ctor_Then_Empty()
  {
    Storage storage = new(slots: []);
    Assert.Empty(storage.Party);
  }

  [Fact(DisplayName = "It should ignore Pokémon duplicates.")]
  public void Given_PokemonDuplicate_When_ctor_Then_Ignored()
  {
    KeyValuePair<PokemonId, PokemonSlot> slot = new(PokemonId.NewId(), new PokemonSlot(new Position(0), Box: null));
    Storage storage = new([slot, slot, slot]);
    Assert.Equal(slot.Key, storage.Party.Single());
  }

  [Fact(DisplayName = "It should throw ArgumentException when a Pokémon occupies multiple slots.")]
  public void Given_MultiplePokemonSlots_When_ctor_Then_ArgumentException()
  {
    PokemonId pokemonId = PokemonId.NewId();
    KeyValuePair<PokemonId, PokemonSlot>[] slots =
    [
      new(pokemonId, new PokemonSlot(new Position(0), Box: null)),
      new(pokemonId, new PokemonSlot(new Position(0), new Box(0)))
    ];
    var exception = Assert.Throws<ArgumentException>(() => new Storage(slots));
    Assert.Equal("slots", exception.ParamName);
    Assert.StartsWith($"The Pokémon 'Id={pokemonId}' cannot occupy multiple slots.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentException when a slot is occupied by multiple Pokémon.")]
  public void Given_MultipleSlotPokemon_When_ctor_Then_ArgumentException()
  {
    PokemonSlot slot = new(new Position(0), Box: null);
    KeyValuePair<PokemonId, PokemonSlot>[] slots = [new(PokemonId.NewId(), slot), new(PokemonId.NewId(), slot)];
    var exception = Assert.Throws<ArgumentException>(() => new Storage(slots));
    Assert.Equal("slots", exception.ParamName);
    Assert.StartsWith($"The slot '{slot}' cannot seat multiple Pokémon.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the party has empty slots.")]
  public void Given_EmptyPartySlots_When_ctor_Then_ArgumentException()
  {
    KeyValuePair<PokemonId, PokemonSlot>[] slots =
    [
      new(PokemonId.NewId(), new PokemonSlot(new Position(0), Box: null)),
      new(PokemonId.NewId(), new PokemonSlot(new Position(1), Box: null)),
      new(PokemonId.NewId(), new PokemonSlot(new Position(3), Box: null))
    ];
    var exception = Assert.Throws<ArgumentException>(() => new Storage(slots));
    Assert.Equal("slots", exception.ParamName);
    Assert.StartsWith("The party cannot have empty slots between occupied slots.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the party is empty, but not boxes.")]
  public void Given_PartyEmptyNotBoxes_When_ctor_Then_ArgumentException()
  {
    KeyValuePair<PokemonId, PokemonSlot>[] slots =
    [
      new(PokemonId.NewId(), new PokemonSlot(new Position(0), new Box(0)))
    ];
    var exception = Assert.Throws<ArgumentException>(() => new Storage(slots));
    Assert.Equal("slots", exception.ParamName);
    Assert.StartsWith("The Pokémon party cannot be empty when there are Pokémon in boxes.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the party size limit is exceeded.")]
  public void Given_PartySizeExceeded_When_ctor_Then_ArgumentException()
  {
    List<KeyValuePair<PokemonId, PokemonSlot>> slots = new(capacity: Storage.PartySize + 1);
    for (int i = 0; i < (Storage.PartySize + 1); i++)
    {
      slots.Add(new(PokemonId.NewId(), new PokemonSlot(new Position(i), Box: null)));
    }
    var exception = Assert.Throws<ArgumentException>(() => new Storage(slots));
    Assert.Equal("slots", exception.ParamName);
    Assert.StartsWith("The party cannot exceed 6 Pokémon.", exception.Message);
  }
}
