using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;
using PokeGame.Infrastructure.Data;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record TechnicalMachinePublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class TechnicalMachinePublishedHandler : INotificationHandler<TechnicalMachinePublished>
{
  private readonly PokemonContext _context;

  public TechnicalMachinePublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(TechnicalMachinePublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    ItemEntity? technicalMachine = await _context.Items.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (technicalMachine is null)
    {
      technicalMachine = new ItemEntity(published);

      _context.Items.Add(technicalMachine);
    }
    else
    {
      technicalMachine.Update(published);
    }

    Guid moveId = published.Invariant.FindRelatedContentValue(TechnicalMachines.Move).Single();
    MoveEntity move = await _context.Moves.SingleOrDefaultAsync(x => x.Id == moveId, cancellationToken)
      ?? throw new InvalidOperationException($"The move entity 'Id={moveId}' was not found.");
    technicalMachine.SetMove(move);

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record TechnicalMachineUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class TechnicalMachineUnpublishedHandler : INotificationHandler<TechnicalMachineUnpublished>
{
  private readonly PokemonContext _context;

  public TechnicalMachineUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(TechnicalMachineUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Items.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
