using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Events;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class FormEvents : IEventHandler<FormCreated>,
  IEventHandler<FormDeleted>,
  IEventHandler<FormUniqueNameChanged>,
  IEventHandler<FormUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddScoped<IEventHandler<FormCreated>, FormEvents>();
    services.AddScoped<IEventHandler<FormDeleted>, FormEvents>();
    services.AddScoped<IEventHandler<FormUniqueNameChanged>, FormEvents>();
    services.AddScoped<IEventHandler<FormUpdated>, FormEvents>();
  }

  private readonly PokemonContext _context;
  private readonly ILogger<FormEvents> _logger;

  public FormEvents(PokemonContext context, ILogger<FormEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task HandleAsync(FormCreated @event, CancellationToken cancellationToken)
  {
    FormEntity? form = await _context.Forms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (form is not null)
    {
      _logger.LogUnexpectedVersion(@event, form);
      return;
    }

    VarietyEntity variety = await _context.Varieties.Include(x => x.Species)
      .SingleOrDefaultAsync(x => x.StreamId == @event.VarietyId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The variety entity 'StreamId={@event.VarietyId}' was not found");

    form = new FormEntity(variety, @event);
    _context.Forms.Add(form);

    await SetAbilitiesAsync(form, @event.Abilities, cancellationToken);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(FormDeleted @event, CancellationToken cancellationToken)
  {
    FormEntity? form = await _context.Forms
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (form is null)
    {
      _logger.LogUnexpectedVersion(@event, form);
      return;
    }

    _context.Forms.Remove(form);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(FormUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    FormEntity? form = await _context.Forms
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (form is null || (form.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, form);
      return;
    }

    form.SetUniqueName(@event);

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  public async Task HandleAsync(FormUpdated @event, CancellationToken cancellationToken)
  {
    FormEntity? form = await _context.Forms
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (form is null || (form.Version != (@event.Version - 1)))
    {
      _logger.LogUnexpectedVersion(@event, form);
      return;
    }

    form.Update(@event);

    if (@event.Abilities is not null)
    {
      await SetAbilitiesAsync(form, @event.Abilities, cancellationToken);
    }

    await _context.SaveChangesAsync(cancellationToken);
    _logger.LogSuccess(@event);
  }

  private async Task SetAbilitiesAsync(FormEntity form, FormAbilities abilities, CancellationToken cancellationToken)
  {
    List<string> streamIds = new(capacity: 3)
    {
      abilities.Primary.Value
    };
    if (abilities.Secondary.HasValue)
    {
      streamIds.Add(abilities.Secondary.Value.Value);
    }
    if (abilities.Hidden.HasValue)
    {
      streamIds.Add(abilities.Hidden.Value.Value);
    }
    Dictionary<string, AbilityEntity> abilitiesById = await _context.Abilities
      .Where(x => streamIds.Contains(x.StreamId))
      .ToDictionaryAsync(x => x.StreamId, x => x, cancellationToken);

    Dictionary<AbilitySlot, AbilityEntity> slots = new(capacity: 3);
    if (abilitiesById.TryGetValue(abilities.Primary.Value, out AbilityEntity? primary))
    {
      slots[AbilitySlot.Primary] = primary;
    }
    if (abilities.Secondary.HasValue && abilitiesById.TryGetValue(abilities.Secondary.Value.Value, out AbilityEntity? secondary))
    {
      slots[AbilitySlot.Secondary] = secondary;
    }
    if (abilities.Hidden.HasValue && abilitiesById.TryGetValue(abilities.Hidden.Value.Value, out AbilityEntity? hidden))
    {
      slots[AbilitySlot.Hidden] = hidden;
    }

    foreach (FormAbilityEntity ability in form.Abilities)
    {
      if (!slots.ContainsKey(ability.Slot))
      {
        _context.FormAbilities.Remove(ability);
      }
    }
    foreach (KeyValuePair<AbilitySlot, AbilityEntity> ability in slots)
    {
      form.SetAbility(ability.Key, ability.Value);
    }
  }
}
