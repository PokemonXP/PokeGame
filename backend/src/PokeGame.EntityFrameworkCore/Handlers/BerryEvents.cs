using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record BerryPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class BerryPublishedHandler : INotificationHandler<BerryPublished>
{
  private readonly PokemonContext _context;

  public BerryPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(BerryPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    ItemEntity? berry = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (berry is null)
    {
      berry = new ItemEntity(published);

      _context.Items.Add(berry);
    }
    else
    {
      berry.Update(published);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record BerryUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class BerryUnpublishedHandler : INotificationHandler<BerryUnpublished>
{
  private readonly PokemonContext _context;

  public BerryUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(BerryUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Items.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
