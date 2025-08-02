using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Battles.Commands;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Battles.Queries;

namespace PokeGame.Core.Battles;

public interface IBattleService
{
  Task<BattleModel?> CancelAsync(Guid id, CancellationToken cancellationToken = default);
  Task<BattleModel> CreateAsync(CreateBattlePayload payload, CancellationToken cancellationToken = default);
  Task<BattleModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<BattleModel?> EscapeAsync(Guid id, CancellationToken cancellationToken = default);
  Task<BattleModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<BattleModel?> ResetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<BattleModel>> SearchAsync(SearchBattlesPayload payload, CancellationToken cancellationToken = default);
  Task<BattleModel?> StartAsync(Guid id, CancellationToken cancellationToken = default);
  Task<BattleModel?> SwitchPokemonAsync(Guid id, SwitchBattlePokemonPayload payload, CancellationToken cancellationToken = default);
  Task<BattleModel?> UpdateAsync(Guid id, UpdateBattlePayload payload, CancellationToken cancellationToken = default);
  Task<BattleModel?> UseMoveAsync(Guid id, UseBattleMovePayload payload, CancellationToken cancellationToken = default);
}

internal class BattleService : IBattleService
{
  public static void RegisterServices(IServiceCollection services)
  {
    services.AddTransient<IBattleService, BattleService>();
    services.AddTransient<IBattleManager, BattleManager>();
    services.AddTransient<ICommandHandler<CancelBattle, BattleModel?>, CancelBattleHandler>();
    services.AddTransient<ICommandHandler<CreateBattle, BattleModel>, CreateBattleHandler>();
    services.AddTransient<ICommandHandler<DeleteBattle, BattleModel?>, DeleteBattleHandler>();
    services.AddTransient<ICommandHandler<EscapeBattle, BattleModel?>, EscapeBattleHandler>();
    services.AddTransient<ICommandHandler<ResetBattle, BattleModel?>, ResetBattleHandler>();
    services.AddTransient<ICommandHandler<StartBattle, BattleModel?>, StartBattleHandler>();
    services.AddTransient<ICommandHandler<SwitchBattlePokemon, BattleModel?>, SwitchBattlePokemonHandler>();
    services.AddTransient<ICommandHandler<UpdateBattle, BattleModel?>, UpdateBattleHandler>();
    services.AddTransient<ICommandHandler<UseBattleMove, BattleModel?>, UseBattleMoveHandler>();
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

  public async Task<BattleModel?> CancelAsync(Guid id, CancellationToken cancellationToken)
  {
    CancelBattle command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
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

  public async Task<BattleModel?> EscapeAsync(Guid id, CancellationToken cancellationToken)
  {
    EscapeBattle command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<BattleModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadBattle query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<BattleModel?> ResetAsync(Guid id, CancellationToken cancellationToken)
  {
    ResetBattle command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
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

  public async Task<BattleModel?> SwitchPokemonAsync(Guid id, SwitchBattlePokemonPayload payload, CancellationToken cancellationToken)
  {
    SwitchBattlePokemon command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<BattleModel?> UpdateAsync(Guid id, UpdateBattlePayload payload, CancellationToken cancellationToken)
  {
    UpdateBattle command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<BattleModel?> UseMoveAsync(Guid id, UseBattleMovePayload payload, CancellationToken cancellationToken)
  {
    UseBattleMove command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
