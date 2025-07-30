using Krakenar.Core;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Queries;

internal record ReadBattle(Guid Id) : IQuery<BattleModel?>;

internal class ReadBattleHandler : IQueryHandler<ReadBattle, BattleModel?>
{
  private readonly IBattleQuerier _battleQuerier;

  public ReadBattleHandler(IBattleQuerier battleQuerier)
  {
    _battleQuerier = battleQuerier;
  }

  public async Task<BattleModel?> HandleAsync(ReadBattle query, CancellationToken cancellationToken)
  {
    return await _battleQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
