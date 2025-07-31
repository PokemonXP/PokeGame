namespace PokeGame.Core.Pokemon;

public class Storage
{
  public const int PartySize = 6;
  public const int BoxSize = 5 * 6;

  private readonly Dictionary<PokemonId, PokemonSlot> _pokemon = [];
  public IReadOnlyDictionary<PokemonId, PokemonSlot> Pokemon => _pokemon.AsReadOnly();

  private readonly Dictionary<PokemonSlot, PokemonId> _slots = [];
  public IReadOnlyDictionary<PokemonSlot, PokemonId> Slots => _slots.AsReadOnly();

  private readonly List<PokemonId> _party = new(capacity: PartySize);
  public IReadOnlyCollection<PokemonId> Party => _party.AsReadOnly();

  private readonly Dictionary<int, Dictionary<int, PokemonId>> _boxes = [];
  public IReadOnlyDictionary<Box, IReadOnlyDictionary<Position, PokemonId>> Boxes
  {
    get
    {
      Dictionary<Box, IReadOnlyDictionary<Position, PokemonId>> boxes = new(capacity: _boxes.Count);
      foreach (KeyValuePair<int, Dictionary<int, PokemonId>> box in _boxes)
      {
        boxes[new Box(box.Key)] = box.Value.ToDictionary(x => new Position(x.Key), x => x.Value).AsReadOnly();
      }
      return boxes.AsReadOnly();
    }
  }

  public Storage(IEnumerable<KeyValuePair<PokemonId, PokemonSlot>> slots)
  {
    List<KeyValuePair<PokemonId, PokemonSlot>> party = [];
    foreach (KeyValuePair<PokemonId, PokemonSlot> slot in slots)
    {
      PokemonId pokemonId = slot.Key;
      if (_pokemon.TryGetValue(pokemonId, out PokemonSlot? existingSlot))
      {
        if (existingSlot != slot.Value)
        {
          throw new ArgumentException($"The Pokémon 'Id={pokemonId}' cannot occupy multiple slots.", nameof(slots));
        }
        continue;
      }
      if (_slots.TryGetValue(slot.Value, out PokemonId existingPokemonId))
      {
        if (existingPokemonId != pokemonId)
        {
          throw new ArgumentException($"The slot '{slot.Value}' cannot seat multiple Pokémon.", nameof(slots));
        }
        continue;
      }

      _pokemon[pokemonId] = slot.Value;
      _slots[slot.Value] = pokemonId;

      int position = slot.Value.Position.Value;
      int? box = slot.Value.Box?.Value;
      if (box.HasValue)
      {
        if (!_boxes.TryGetValue(box.Value, out Dictionary<int, PokemonId>? boxPokemon))
        {
          boxPokemon = [];
          _boxes[box.Value] = boxPokemon;
        }
        boxPokemon[position] = pokemonId;
      }
      else
      {
        party.Add(slot);
      }
    }

    if (party.Count > 0)
    {
      if (party.Count > PartySize)
      {
        throw new ArgumentException($"The party cannot exceed {PartySize} Pokémon.", nameof(slots));
      }

      int maximum = party.Max(x => x.Value.Position.Value);
      if (maximum != (party.Count - 1))
      {
        throw new ArgumentException("The party cannot have empty slots between occupied slots.", nameof(slots));
      }

      _party.AddRange(party.OrderBy(x => x.Value.Position.Value).Select(x => x.Key));
    }
    else if (_pokemon.Count > 0)
    {
      throw new ArgumentException("The Pokémon party cannot be empty when there are Pokémon in boxes.", nameof(slots));
    }
  }

  public PokemonSlot GetFirstEmptySlot()
  {
    Position? position = GetFirstPartyEmptySlot();
    return position is not null ? new PokemonSlot(position) : GetFirstBoxEmptySlot();
  }
  public Position? GetFirstPartyEmptySlot() => _party.Count < PartySize ? new Position(_party.Count) : null;
  public PokemonSlot GetFirstBoxEmptySlot()
  {
    int box = 0;
    while (true)
    {
      if (!_boxes.TryGetValue(box, out Dictionary<int, PokemonId>? boxPokemon))
      {
        return new PokemonSlot(new Position(0), new Box(box));
      }
      else if (boxPokemon.Count < BoxSize)
      {
        for (int i = 0; i < BoxSize; i++)
        {
          if (!boxPokemon.ContainsKey(i))
          {
            return new PokemonSlot(new Position(i), new Box(box));
          }
        }
      }
      box++;
    }
  }

  public bool IsEmpty(PokemonSlot slot) => !_slots.ContainsKey(slot);
}
