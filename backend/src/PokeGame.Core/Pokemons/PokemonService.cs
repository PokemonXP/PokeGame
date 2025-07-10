using Krakenar.Core;
using PokeGame.Core.Pokemons.Commands;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Pokemons.Queries;

namespace PokeGame.Core.Pokemons;

public interface IPokemonService
{
  Task<PokemonModel> CreateAsync(CreatePokemonPayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
}

internal class PokemonService : IPokemonService
{
  private readonly ICommandHandler<CreatePokemon, PokemonModel> _createPokemon;
  private readonly IQueryHandler<ReadPokemon, PokemonModel?> _readPokemon;

  public PokemonService(
    ICommandHandler<CreatePokemon, PokemonModel> createPokemon,
    IQueryHandler<ReadPokemon, PokemonModel?> readPokemon)
  {
    _createPokemon = createPokemon;
    _readPokemon = readPokemon;
  }

  public async Task<PokemonModel> CreateAsync(CreatePokemonPayload payload, CancellationToken cancellationToken)
  {
    CreatePokemon command = new(payload);
    return await _createPokemon.HandleAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadPokemon query = new(id, uniqueName);
    return await _readPokemon.HandleAsync(query, cancellationToken);
  }
}
