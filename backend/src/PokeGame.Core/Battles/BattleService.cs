using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Battles.Commands;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles;

public interface IBattleService
{
  Task<BattleModel> CreateAsync(CreateBattlePayload payload, CancellationToken cancellationToken = default);
}

internal class BattleService : IBattleService
{
  public static void RegisterServices(IServiceCollection services)
  {
    services.AddTransient<IBattleService, BattleService>();
    services.AddTransient<ICommandHandler<CreateBattle, BattleModel>, CreateBattleHandler>();
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
}
