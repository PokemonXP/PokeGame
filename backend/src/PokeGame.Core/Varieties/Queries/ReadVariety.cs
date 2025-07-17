using Krakenar.Contracts;
using Krakenar.Core;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Varieties.Queries;

internal record ReadVariety(Guid? Id, string? UniqueName) : IQuery<VarietyModel?>;

/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadVarietyHandler : IQueryHandler<ReadVariety, VarietyModel?>
{
  private readonly IVarietyQuerier _varietyQuerier;

  public ReadVarietyHandler(IVarietyQuerier varietyQuerier)
  {
    _varietyQuerier = varietyQuerier;
  }

  public async Task<VarietyModel?> HandleAsync(ReadVariety query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, VarietyModel> varieties = new(capacity: 2);

    if (query.Id.HasValue)
    {
      VarietyModel? variety = await _varietyQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (variety is not null)
      {
        varieties[variety.Id] = variety;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      VarietyModel? variety = await _varietyQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (variety is not null)
      {
        varieties[variety.Id] = variety;
      }
    }

    if (varieties.Count > 1)
    {
      throw TooManyResultsException<VarietyModel>.ExpectedSingle(varieties.Count);
    }

    return varieties.SingleOrDefault().Value;
  }
}
