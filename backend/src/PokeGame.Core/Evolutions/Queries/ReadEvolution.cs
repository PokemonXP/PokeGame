using Krakenar.Core;
using PokeGame.Core.Evolutions.Models;

namespace PokeGame.Core.Evolutions.Queries;

internal record ReadEvolution(Guid Id) : IQuery<EvolutionModel?>;

internal class ReadEvolutionHandler : IQueryHandler<ReadEvolution, EvolutionModel?>
{
  private readonly IEvolutionQuerier _evolutionQuerier;

  public ReadEvolutionHandler(IEvolutionQuerier evolutionQuerier)
  {
    _evolutionQuerier = evolutionQuerier;
  }

  public async Task<EvolutionModel?> HandleAsync(ReadEvolution query, CancellationToken cancellationToken)
  {
    return await _evolutionQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
