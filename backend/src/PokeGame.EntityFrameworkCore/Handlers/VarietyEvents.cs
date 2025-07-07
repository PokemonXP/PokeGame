using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record VarietyPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class VarietyPublishedHandler : INotificationHandler<VarietyPublished>
{
  private readonly PokemonContext _context;

  public VarietyPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(VarietyPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    VarietyEntity? variety = await _context.Varieties.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (variety is null)
    {
      variety = new VarietyEntity(published);

      _context.Varieties.Add(variety);
    }
    else
    {
      variety.Update(published);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record VarietyUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class VarietyUnpublishedHandler : INotificationHandler<VarietyUnpublished>
{
  private readonly PokemonContext _context;

  public VarietyUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(VarietyUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Varieties.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
