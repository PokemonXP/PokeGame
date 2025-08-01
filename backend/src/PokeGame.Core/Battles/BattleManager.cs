using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles;

internal interface IBattleManager
{
  Task<IReadOnlyDictionary<string, Specimen>> FindPokemonAsync(IEnumerable<string> idOrUniqueNames, string propertyName, CancellationToken cancellationToken);
}

internal class BattleManager : IBattleManager
{
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public BattleManager(IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<IReadOnlyDictionary<string, Specimen>> FindPokemonAsync(IEnumerable<string> idOrUniqueNames, string propertyName, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<PokemonKey> keys = await _pokemonQuerier.GetKeysAsync(cancellationToken);
    Dictionary<Guid, PokemonId> pokemonByIds = new(capacity: keys.Count);
    Dictionary<string, PokemonId> pokemonByNames = new(capacity: keys.Count);
    foreach (PokemonKey key in keys)
    {
      pokemonByIds[key.Id] = key.PokemonId;
      pokemonByNames[Normalize(key.UniqueName)] = key.PokemonId;
    }

    int capacity = idOrUniqueNames.Count();
    Dictionary<string, PokemonId> foundPokemon = new(capacity);
    HashSet<string> missingPokemon = new(capacity);
    foreach (string idOrUniqueName in idOrUniqueNames)
    {
      if ((Guid.TryParse(idOrUniqueName, out Guid id) && pokemonByIds.TryGetValue(id, out PokemonId pokemonId))
        || pokemonByNames.TryGetValue(Normalize(idOrUniqueName), out pokemonId))
      {
        foundPokemon[idOrUniqueName] = pokemonId;
      }
      else
      {
        missingPokemon.Add(idOrUniqueName);
      }
    }
    if (missingPokemon.Count > 0)
    {
      throw new PokemonNotFoundException(missingPokemon, propertyName);
    }

    Dictionary<PokemonId, Specimen> pokemon = (await _pokemonRepository.LoadAsync(foundPokemon.Values, cancellationToken)).ToDictionary(x => x.Id, x => x);
    return foundPokemon.ToDictionary(x => x.Key, x => pokemon[x.Value]).AsReadOnly();
  }
  private static string Normalize(string value) => value.Trim().ToLowerInvariant();
}
