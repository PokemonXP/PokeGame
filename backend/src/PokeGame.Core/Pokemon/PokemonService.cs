using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Pokemon.Commands;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemon.Queries;

namespace PokeGame.Core.Pokemon;

public interface IPokemonService
{
  Task<PokemonModel> CreateAsync(CreatePokemonPayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<PokemonModel>> SearchAsync(SearchPokemonPayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> UpdateAsync(Guid id, UpdatePokemonPayload payload, CancellationToken cancellationToken = default);

  Task<PokemonModel?> RememberMoveAsync(Guid pokemonId, Guid moveId, RememberPokemonMovePayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> SwitchMovesAsync(Guid id, SwitchPokemonMovesPayload payload, CancellationToken cancellationToken = default);

  Task<PokemonModel?> CatchAsync(Guid id, CatchPokemonPayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReceiveAsync(Guid id, ReceivePokemonPayload payload, CancellationToken cancellationToken = default);
  Task<PokemonModel?> ReleaseAsync(Guid id, CancellationToken cancellationToken = default);

  Task<PokemonModel?> DepositAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PokemonModel?> WithdrawAsync(Guid id, CancellationToken cancellationToken = default);
}

internal class PokemonService : IPokemonService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IPokemonService, PokemonService>();
    services.AddTransient<IPokemonManager, PokemonManager>();
    services.AddTransient<ICommandHandler<CatchPokemon, PokemonModel?>, CatchPokemonHandler>();
    services.AddTransient<ICommandHandler<CreatePokemon, PokemonModel>, CreatePokemonHandler>();
    services.AddTransient<ICommandHandler<DeletePokemon, PokemonModel?>, DeletePokemonHandler>();
    services.AddTransient<ICommandHandler<DepositPokemon, PokemonModel?>, DepositPokemonHandler>();
    services.AddTransient<ICommandHandler<ReceivePokemon, PokemonModel?>, ReceivePokemonHandler>();
    services.AddTransient<ICommandHandler<ReleasePokemon, PokemonModel?>, ReleasePokemonHandler>();
    services.AddTransient<ICommandHandler<RememberPokemonMove, PokemonModel?>, RememberPokemonMoveHandler>();
    services.AddTransient<ICommandHandler<SwitchPokemonMoves, PokemonModel?>, SwitchPokemonMovesHandler>();
    services.AddTransient<ICommandHandler<UpdatePokemon, PokemonModel?>, UpdatePokemonHandler>();
    services.AddTransient<ICommandHandler<WithdrawPokemon, PokemonModel?>, WithdrawPokemonHandler>();
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

  public async Task<PokemonModel?> RememberMoveAsync(Guid pokemonId, Guid moveId, RememberPokemonMovePayload payload, CancellationToken cancellationToken)
  {
    RememberPokemonMove command = new(pokemonId, moveId, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> SwitchMovesAsync(Guid id, SwitchPokemonMovesPayload payload, CancellationToken cancellationToken)
  {
    SwitchPokemonMoves command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> CatchAsync(Guid id, CatchPokemonPayload payload, CancellationToken cancellationToken)
  {
    CatchPokemon command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> ReceiveAsync(Guid id, ReceivePokemonPayload payload, CancellationToken cancellationToken)
  {
    ReceivePokemon command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> ReleaseAsync(Guid id, CancellationToken cancellationToken)
  {
    ReleasePokemon command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> DepositAsync(Guid id, CancellationToken cancellationToken)
  {
    DepositPokemon command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<PokemonModel?> WithdrawAsync(Guid id, CancellationToken cancellationToken)
  {
    WithdrawPokemon command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
