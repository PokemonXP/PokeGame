using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Pokemons.Commands;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Pokemons.Queries;

namespace PokeGame.Core.Pokemons;

public interface IPokemonService
{
  Task<PokemonModel> CreateAsync(CreatePokemonPayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<PokemonModel>> SearchAsync(SearchPokemonPayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> UpdateAsync(Guid id, UpdatePokemonPayload payload, CancellationToken cancellationToken = default);

  Task<PokemonModel?> RelearnMoveAsync(Guid id, RelearnPokemonMovePayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> SwitchMovesAsync(Guid id, SwitchPokemonMovesPayload payload, CancellationToken cancellationToken = default);
}

internal class PokemonService : IPokemonService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IPokemonService, PokemonService>();
    services.AddTransient<IPokemonManager, PokemonManager>();
    services.AddTransient<ICommandHandler<CreatePokemon, PokemonModel>, CreatePokemonHandler>();
    services.AddTransient<ICommandHandler<DeletePokemon, PokemonModel?>, DeletePokemonHandler>();
    services.AddTransient<ICommandHandler<RelearnPokemonMove, PokemonModel?>, RelearnPokemonMoveHandler>();
    services.AddTransient<ICommandHandler<SwitchPokemonMoves, PokemonModel?>, SwitchPokemonMovesHandler>();
    services.AddTransient<ICommandHandler<UpdatePokemon, PokemonModel?>, UpdatePokemonHandler>();
    services.AddTransient<IQueryHandler<ReadPokemon, PokemonModel?>, ReadPokemonHandler>();
    services.AddTransient<IQueryHandler<SearchPokemon, SearchResults<PokemonModel>>, SearchPokemonHandler>();
  }

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

  public async Task<PokemonModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeletePokemon command = new(id);
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

  public async Task<PokemonModel?> UpdateAsync(Guid id, UpdatePokemonPayload payload, CancellationToken cancellationToken)
  {
    UpdatePokemon command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> RelearnMoveAsync(Guid id, RelearnPokemonMovePayload payload, CancellationToken cancellationToken)
  {
    RelearnPokemonMove command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> SwitchMovesAsync(Guid id, SwitchPokemonMovesPayload payload, CancellationToken cancellationToken)
  {
    SwitchPokemonMoves command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
