using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core;
using PokeGame.EntityFrameworkCore.Entities;
using PokeGame.Infrastructure.Data;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal record FormPublished(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : INotification;

internal class FormPublishedHandler : INotificationHandler<FormPublished>
{
  private readonly PokemonContext _context;

  public FormPublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(FormPublished published, CancellationToken cancellationToken)
  {
    string streamId = published.Event.StreamId.Value;
    FormEntity? form = await _context.Forms
      .Include(x => x.Abilities)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (form is null)
    {
      Guid varietyId = published.Invariant.FindRelatedContentValue(Forms.Variety).Single();
      VarietyEntity variety = await _context.Varieties.SingleOrDefaultAsync(x => x.Id == varietyId, cancellationToken)
        ?? throw new InvalidOperationException($"The variety entity 'Id={varietyId}' was not found.");

      form = new FormEntity(variety, published);

      _context.Forms.Add(form);
    }
    else
    {
      form.Update(published);
    }

    IReadOnlyDictionary<AbilitySlot, AbilityEntity> abilities = await GetAbilitiesAsync(published.Invariant, cancellationToken);
    foreach (FormAbilityEntity ability in form.Abilities)
    {
      if (!abilities.ContainsKey(ability.Slot))
      {
        _context.FormAbilities.Remove(ability);
      }
    }
    foreach (KeyValuePair<AbilitySlot, AbilityEntity> ability in abilities)
    {
      form.SetAbility(ability.Key, ability.Value);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }

  private async Task<IReadOnlyDictionary<AbilitySlot, AbilityEntity>> GetAbilitiesAsync(ContentLocale form, CancellationToken cancellationToken)
  {
    Guid primaryId = form.FindRelatedContentValue(Forms.PrimaryAbility).Single();
    Guid? secondaryId = form.TryGetRelatedContentValue(Forms.SecondaryAbility)?.Single();
    Guid? hiddenId = form.TryGetRelatedContentValue(Forms.HiddenAbility)?.Single();
    List<Guid> ids = new(capacity: 3)
    {
      primaryId
    };
    if (secondaryId.HasValue)
    {
      ids.Add(secondaryId.Value);
    }
    if (hiddenId.HasValue)
    {
      ids.Add(hiddenId.Value);
    }

    Dictionary<Guid, AbilityEntity> abilities = await _context.Abilities.Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x, cancellationToken);
    Dictionary<AbilitySlot, AbilityEntity> slots = new(capacity: 3);
    if (abilities.TryGetValue(primaryId, out AbilityEntity? primaryAbility))
    {
      slots[AbilitySlot.Primary] = primaryAbility;
    }
    if (secondaryId.HasValue && abilities.TryGetValue(secondaryId.Value, out AbilityEntity? secondaryAbility))
    {
      slots[AbilitySlot.Secondary] = secondaryAbility;
    }
    if (hiddenId.HasValue && abilities.TryGetValue(hiddenId.Value, out AbilityEntity? hiddenAbility))
    {
      slots[AbilitySlot.Hidden] = hiddenAbility;
    }
    return slots.AsReadOnly();
  }
}

internal record FormUnpublished(ContentLocaleUnpublished Event) : INotification;

internal class FormUnpublishedHandler : INotificationHandler<FormUnpublished>
{
  private readonly PokemonContext _context;

  public FormUnpublishedHandler(PokemonContext context)
  {
    _context = context;
  }

  public async Task Handle(FormUnpublished unpublished, CancellationToken cancellationToken)
  {
    string streamId = unpublished.Event.StreamId.Value;
    await _context.Forms.Where(x => x.StreamId == streamId).ExecuteDeleteAsync(cancellationToken);
  }
}
