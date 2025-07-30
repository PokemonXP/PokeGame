using Krakenar.Core;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Commands;

internal record DeleteBattle(Guid Id) : ICommand<BattleModel?>;

internal class DeleteBattleHandler : ICommandHandler<DeleteBattle, BattleModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;

  public DeleteBattleHandler(IApplicationContext applicationContext, IBattleQuerier battleQuerier, IBattleRepository battleRepository)
  {
    _applicationContext = applicationContext;
    _battleQuerier = battleQuerier;
    _battleRepository = battleRepository;
  }

  public async Task<BattleModel?> HandleAsync(DeleteBattle command, CancellationToken cancellationToken)
  {
    BattleId battleId = new(command.Id);
    Battle? battle = await _battleRepository.LoadAsync(battleId, cancellationToken);
    if (battle is null)
    {
      return null;
    }
    BattleModel model = await _battleQuerier.ReadAsync(battle, cancellationToken);

    battle.Delete(_applicationContext.ActorId);
    await _battleRepository.SaveAsync(battle, cancellationToken);

    return model;
  }
}
