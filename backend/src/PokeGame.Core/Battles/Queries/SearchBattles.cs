using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Core.Battles.Queries;

internal record SearchBattles(SearchBattlesPayload Payload) : IQuery<SearchResults<BattleModel>>;

internal class SearchBattlesHandler : IQueryHandler<SearchBattles, SearchResults<BattleModel>>
{
  private readonly IBattleQuerier _battleQuerier;

  public SearchBattlesHandler(IBattleQuerier battleQuerier)
  {
    _battleQuerier = battleQuerier;
  }

  public async Task<SearchResults<BattleModel>> HandleAsync(SearchBattles query, CancellationToken cancellationToken)
  {
    return await _battleQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
