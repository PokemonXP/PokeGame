using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Battles.Commands;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Battles.Queries;

namespace PokeGame.Core.Battles;

public interface IBattleService
{
  Task<BattleModel> CreateAsync(CreateBattlePayload payload, CancellationToken cancellationToken = default);
  Task<BattleModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<BattleModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<BattleModel>> SearchAsync(SearchBattlesPayload payload, CancellationToken cancellationToken = default);
  Task<BattleModel?> StartAsync(Guid id, CancellationToken cancellationToken = default);
  Task<BattleModel?> UpdateAsync(Guid id, UpdateBattlePayload payload, CancellationToken cancellationToken = default);
}

internal class BattleService : IBattleService
{
  public static void RegisterServices(IServiceCollection services)
  {
    services.AddTransient<IBattleService, BattleService>();
    services.AddTransient<ICommandHandler<CreateBattle, BattleModel>, CreateBattleHandler>();
    services.AddTransient<ICommandHandler<DeleteBattle, BattleModel?>, DeleteBattleHandler>();
    services.AddTransient<ICommandHandler<StartBattle, BattleModel?>, StartBattleHandler>();
    services.AddTransient<ICommandHandler<UpdateBattle, BattleModel?>, UpdateBattleHandler>();
    services.AddTransient<IQueryHandler<ReadBattle, BattleModel?>, ReadBattleHandler>();
    services.AddTransient<IQueryHandler<SearchBattles, SearchResults<BattleModel>>, SearchBattlesHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public BattleService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<BattleModel> CreateAsync(CreateBattlePayload payload, CancellationToken cancellationToken)
  {
    CreateBattle command = new(payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<BattleModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteBattle command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<BattleModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadBattle query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<BattleModel>> SearchAsync(SearchBattlesPayload payload, CancellationToken cancellationToken)
  {
    SearchBattles query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<BattleModel?> StartAsync(Guid id, CancellationToken cancellationToken)
  {
    StartBattle command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<BattleModel?> UpdateAsync(Guid id, UpdateBattlePayload payload, CancellationToken cancellationToken)
  {
    UpdateBattle command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
