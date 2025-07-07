using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record AbilityPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class AbilityPublishedHandler : INotificationHandler<AbilityPublished>
{
  private readonly PokemonContext _context;

  public AbilityPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(AbilityPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    AbilityEntity? ability = await _context.Abilities.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (ability is null)
    {
      ability = new AbilityEntity(published);

      _context.Abilities.Add(ability);
    }
    else
    {
      ability.Update(published);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}

internal record AbilityUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class AbilityUnpublishedHandler : INotificationHandler<AbilityUnpublished>
{
  private readonly PokemonContext _context;

  public AbilityUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(AbilityUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Abilities.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
