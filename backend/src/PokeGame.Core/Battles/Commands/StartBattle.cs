using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Commands;

internal record StartBattle(Guid Id) : ICommand<BattleModel?>;

internal class StartBattleHandler : ICommandHandler<StartBattle, BattleModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;

  public StartBattleHandler(IApplicationContext applicationContext, IBattleQuerier battleQuerier, IBattleRepository battleRepository)
  {
    _applicationContext = applicationContext;
    _battleQuerier = battleQuerier;
    _battleRepository = battleRepository;
  }

  public async Task<BattleModel?> HandleAsync(StartBattle command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    BattleId battleId = new(command.Id);
    Battle? battle = await _battleRepository.LoadAsync(battleId, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    battle.Start(actorId);

    await _battleRepository.SaveAsync(battle, cancellationToken);

    return await _battleQuerier.ReadAsync(battle, cancellationToken);
  }
}
