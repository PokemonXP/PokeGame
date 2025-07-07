using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

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

    // TODO(fpion): Regional Numbers

    await _context.SaveChangesAsync(cancellationToken);
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
