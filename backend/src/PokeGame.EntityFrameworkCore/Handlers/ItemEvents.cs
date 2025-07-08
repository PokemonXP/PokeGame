using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record ItemPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class ItemPublishedHandler : INotificationHandler<ItemPublished>
{
  private readonly PokemonContext _context;

  public ItemPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(ItemPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    ItemEntity? item = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (item is null)
    {
      item = new ItemEntity(published);

      _context.Items.Add(item);
    }
    else
    {
      item.Update(published);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record ItemUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class ItemUnpublishedHandler : INotificationHandler<ItemUnpublished>
{
  private readonly PokemonContext _context;

  public ItemUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(ItemUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Items.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
