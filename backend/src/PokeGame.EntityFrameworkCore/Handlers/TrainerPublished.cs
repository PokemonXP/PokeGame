using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record TrainerPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class TrainerPublishedHandler : INotificationHandler<TrainerPublished>
{
  private readonly PokemonContext _context;

  public TrainerPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(TrainerPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    TrainerEntity? Trainer = await _context.Trainers.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (Trainer is null)
    {
      Trainer = new TrainerEntity(published);

      _context.Trainers.Add(Trainer);
    }
    else
    {
      Trainer.Update(published);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record TrainerUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class TrainerUnpublishedHandler : INotificationHandler<TrainerUnpublished>
{
  private readonly PokemonContext _context;

  public TrainerUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(TrainerUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Trainers.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
