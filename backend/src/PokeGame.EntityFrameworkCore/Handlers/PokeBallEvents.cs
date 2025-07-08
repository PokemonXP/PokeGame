using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record PokeBallPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class PokeBallPublishedHandler : INotificationHandler<PokeBallPublished>
{
  private readonly PokemonContext _context;

  public PokeBallPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(PokeBallPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    ItemEntity? pokeBall = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (pokeBall is null)
    {
      pokeBall = new ItemEntity(published);

      _context.Items.Add(pokeBall);
    }
    else
    {
      pokeBall.Update(published);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record PokeBallUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class PokeBallUnpublishedHandler : INotificationHandler<PokeBallUnpublished>
{
  private readonly PokemonContext _context;

  public PokeBallUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(PokeBallUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Items.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
