using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;
using PokeGame.Infrastructure.Data;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record SpeciesPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class SpeciesPublishedHandler : INotificationHandler<SpeciesPublished>
{
  private readonly PokemonContext _context;

  public SpeciesPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(SpeciesPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    SpeciesEntity? species = await _context.Species
      .Include(x => x.RegionalNumbers)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (species is null)
    {
      species = new SpeciesEntity(published);

      _context.Species.Add(species);
    }
    else
    {
      species.Update(published);
    }

    IReadOnlyDictionary<RegionEntity, int> regionalNumbers = await GetRegionalNumbersAsync(published.Invariant, cancellationToken);
    HashSet<int> regionIds = regionalNumbers.Select(x => x.Key.RegionId).ToHashSet();
    foreach (RegionalNumberEntity regionalNumber in species.RegionalNumbers)
    {
      if (!regionIds.Contains(regionalNumber.RegionId))
      {
        _context.RegionalNumbers.Remove(regionalNumber);
      }
    }
    foreach (KeyValuePair<RegionEntity, int> regionalNumber in regionalNumbers)
    {
      species.SetRegionalNumber(regionalNumber.Key, regionalNumber.Value);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }

  private async Task<IReadOnlyDictionary<RegionEntity, int>> GetRegionalNumbersAsync(ContentLocale invariant, CancellationToken cancellationToken)
  {
    string? fieldValue = invariant.TryGetStringValue(Species.RegionalNumbers);
    if (string.IsNullOrWhiteSpace(fieldValue))
    {
      return new Dictionary<RegionEntity, int>();
    }
    string[] fieldValues = fieldValue.Split('|');

    RegionEntity[] regions = await _context.Regions.ToArrayAsync(cancellationToken);
    int capacity = regions.Length;
    Dictionary<Guid, RegionEntity> regionsById = new(capacity);
    Dictionary<string, RegionEntity> regionsByName = new(capacity);
    foreach (RegionEntity region in regions)
    {
      regionsById[region.Id] = region;
      regionsByName[region.UniqueNameNormalized] = region;
    }

    Dictionary<RegionEntity, int> regionalNumbers = new(capacity);
    foreach (string value in fieldValues)
    {
      string[] values = value.Split(':');
      if (values.Length == 2 && int.TryParse(values.Last(), out int number))
      {
        if ((Guid.TryParse(values.First(), out Guid id) && regionsById.TryGetValue(id, out RegionEntity? region))
          || regionsByName.TryGetValue(Helper.Normalize(values.First()), out region))
        {
          regionalNumbers[region] = number;
        }
      }
    }
    return regionalNumbers.AsReadOnly();
  }
}

internal record SpeciesUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class SpeciesUnpublishedHandler : INotificationHandler<SpeciesUnpublished>
{
  private readonly PokemonContext _context;

  public SpeciesUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(SpeciesUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Species.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
