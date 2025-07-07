using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record MovePublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class MovePublishedHandler : INotificationHandler<MovePublished>
{
  private readonly PokemonContext _context;

  public MovePublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(MovePublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    MoveEntity? move = await _context.Moves.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (move is null)
    {
      move = new MoveEntity(published);

      _context.Moves.Add(move);
    }
    else
    {
      move.Update(published);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record MoveUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class MoveUnpublishedHandler : INotificationHandler<MoveUnpublished>
{
  private readonly PokemonContext _context;

  public MoveUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(MoveUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Moves.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
