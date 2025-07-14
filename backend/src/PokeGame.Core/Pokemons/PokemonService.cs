using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Pokemons.Commands;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Pokemons.Queries;

namespace PokeGame.Core.Pokemons;

public interface IPokemonService
{
  Task<PokemonModel> CreateAsync(CreatePokemonPayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<PokemonModel>> SearchAsync(SearchPokemonPayload payload, CancellationToken cancellationToken = default);
}

internal class PokemonService : IPokemonService
{
  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public PokemonService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<PokemonModel> CreateAsync(CreatePokemonPayload payload, CancellationToken cancellationToken)
  {
    CreatePokemon command = new(payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadPokemon query = new(id, uniqueName);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<PokemonModel>> SearchAsync(SearchPokemonPayload payload, CancellationToken cancellationToken)
  {
    SearchPokemon query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }
}
