using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record BattleItemPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class BattleItemPublishedHandler : INotificationHandler<BattleItemPublished>
{
  private readonly PokemonContext _context;

  public BattleItemPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(BattleItemPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    ItemEntity? battleItem = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (battleItem is null)
    {
      battleItem = new ItemEntity(published);

      _context.Items.Add(battleItem);
    }
    else
    {
      battleItem.Update(published);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record BattleItemUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class BattleItemUnpublishedHandler : INotificationHandler<BattleItemUnpublished>
{
  private readonly PokemonContext _context;

  public BattleItemUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(BattleItemUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Items.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
