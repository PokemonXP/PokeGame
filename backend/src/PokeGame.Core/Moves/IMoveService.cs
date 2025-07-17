using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Moves.Commands;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Moves.Queries;

namespace PokeGame.Core.Moves;

public interface IMoveService
{
  Task<CreateOrReplaceMoveResult> CreateOrReplaceAsync(CreateOrReplaceMovePayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<MoveModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<MoveModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<MoveModel>> SearchAsync(SearchMovesPayload payload, CancellationToken cancellationToken = default);
  Task<MoveModel?> UpdateAsync(Guid id, UpdateMovePayload payload, CancellationToken cancellationToken = default);
}

internal class MoveService : IMoveService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IMoveService, MoveService>();
    services.AddTransient<IMoveManager, MoveManager>();
    services.AddTransient<ICommandHandler<CreateOrReplaceMove, CreateOrReplaceMoveResult>, CreateOrReplaceMoveHandler>();
    services.AddTransient<ICommandHandler<DeleteMove, MoveModel?>, DeleteMoveHandler>();
    services.AddTransient<ICommandHandler<UpdateMove, MoveModel?>, UpdateMoveHandler>();
    services.AddTransient<IQueryHandler<ReadMove, MoveModel?>, ReadMoveHandler>();
    services.AddTransient<IQueryHandler<SearchMoves, SearchResults<MoveModel>>, SearchMovesHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public MoveService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceMoveResult> CreateOrReplaceAsync(CreateOrReplaceMovePayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceMove command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<MoveModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteMove command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<MoveModel?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadMove query = new(id, uniqueName);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<MoveModel>> SearchAsync(SearchMovesPayload payload, CancellationToken cancellationToken)
  {
    SearchMoves query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<MoveModel?> UpdateAsync(Guid id, UpdateMovePayload payload, CancellationToken cancellationToken)
  {
    UpdateMove command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
