using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record RegionPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class RegionPublishedHandler : INotificationHandler<RegionPublished>
{
  private readonly PokemonContext _context;

  public RegionPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(RegionPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    RegionEntity? region = await _context.Regions.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (region is null)
    {
      region = new RegionEntity(published);

      _context.Regions.Add(region);
    }
    else
    {
      region.Update(published);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record RegionUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class RegionUnpublishedHandler : INotificationHandler<RegionUnpublished>
{
  private readonly PokemonContext _context;

  public RegionUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(RegionUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Regions.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
