using Krakenar.Contracts;
using Krakenar.Core;
using PokeGame.Core.Species.Models;

namespace PokeGame.Core.Species.Queries;

internal record ReadSpecies(Guid? Id, string? UniqueName) : IQuery<SpeciesModel?>;

/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadSpeciesHandler : IQueryHandler<ReadSpecies, SpeciesModel?>
{
  private readonly ISpeciesQuerier _speciesQuerier;

  public ReadSpeciesHandler(ISpeciesQuerier speciesQuerier)
  {
    _speciesQuerier = speciesQuerier;
  }

  public async Task<SpeciesModel?> HandleAsync(ReadSpecies query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, SpeciesModel> foundSpecies = new(capacity: 2);

    if (query.Id.HasValue)
    {
      SpeciesModel? species = await _speciesQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (species is not null)
      {
        foundSpecies[species.Id] = species;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      SpeciesModel? species = await _speciesQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (species is not null)
      {
        foundSpecies[species.Id] = species;
      }
    }

    if (foundSpecies.Count > 1)
    {
      throw TooManyResultsException<SpeciesModel>.ExpectedSingle(foundSpecies.Count);
    }

    return foundSpecies.SingleOrDefault().Value;
  }
}
