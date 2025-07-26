using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Evolutions.Models;
using PokeGame.Core.Evolutions.Validators;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;

namespace PokeGame.Core.Evolutions.Commands;

internal record CreateOrReplaceEvolution(CreateOrReplaceEvolutionPayload Payload, Guid? Id) : ICommand<CreateOrReplaceEvolutionResult>;

/// <exception cref="FormNotFoundException"></exception>
/// <exception cref="ItemNotFoundException"></exception>
/// <exception cref="MoveNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceEvolutionHandler : ICommandHandler<CreateOrReplaceEvolution, CreateOrReplaceEvolutionResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IEvolutionQuerier _evolutionQuerier;
  private readonly IEvolutionRepository _evolutionRepository;
  private readonly IFormRepository _formRepository;
  private readonly IItemRepository _itemRepository;
  private readonly IMoveRepository _moveRepository;

  public CreateOrReplaceEvolutionHandler(
    IApplicationContext applicationContext,
    IEvolutionQuerier evolutionQuerier,
    IEvolutionRepository evolutionRepository,
    IFormRepository formRepository,
    IItemRepository itemRepository,
    IMoveRepository moveRepository)
  {
    _applicationContext = applicationContext;
    _evolutionQuerier = evolutionQuerier;
    _evolutionRepository = evolutionRepository;
    _formRepository = formRepository;
    _itemRepository = itemRepository;
    _moveRepository = moveRepository;
  }

  public async Task<CreateOrReplaceEvolutionResult> HandleAsync(CreateOrReplaceEvolution command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    CreateOrReplaceEvolutionPayload payload = command.Payload;
    new CreateOrReplaceEvolutionValidator().ValidateAndThrow(payload);

    EvolutionId evolutionId = EvolutionId.NewId();
    Evolution? evolution = null;
    if (command.Id.HasValue)
    {
      evolutionId = new(command.Id.Value);
      evolution = await _evolutionRepository.LoadAsync(evolutionId, cancellationToken);
    }

    bool created = false;
    if (evolution is null)
    {
      Form source = await _formRepository.LoadAsync(payload.Source, cancellationToken)
        ?? throw new FormNotFoundException(payload.Source, nameof(payload.Source));
      Form target = await _formRepository.LoadAsync(payload.Target, cancellationToken)
        ?? throw new FormNotFoundException(payload.Target, nameof(payload.Target));
      Item? item = string.IsNullOrWhiteSpace(payload.Item) ? null
        : (await _itemRepository.LoadAsync(payload.Item, cancellationToken) ?? throw new ItemNotFoundException(payload.Item, nameof(payload.Item)));

      evolution = new(source, target, payload.Trigger, item, actorId, evolutionId);
      created = true;
    }

    Item? heldItem = string.IsNullOrWhiteSpace(payload.HeldItem) ? null
      : (await _itemRepository.LoadAsync(payload.HeldItem, cancellationToken) ?? throw new ItemNotFoundException(payload.HeldItem, nameof(payload.HeldItem)));
    Move? knownMove = string.IsNullOrWhiteSpace(payload.KnownMove) ? null
      : (await _moveRepository.LoadAsync(payload.KnownMove, cancellationToken) ?? throw new MoveNotFoundException(payload.KnownMove, nameof(payload.KnownMove)));

    evolution.Level = payload.Level < 1 ? null : new Level(payload.Level);
    evolution.Friendship = payload.Friendship;
    evolution.Gender = payload.Gender;
    evolution.HeldItemId = heldItem?.Id;
    evolution.KnownMoveId = knownMove?.Id;
    evolution.Location = string.IsNullOrWhiteSpace(payload.Location) ? null : new Location(payload.Location);
    evolution.TimeOfDay = payload.TimeOfDay;

    evolution.Update(actorId);
    await _evolutionRepository.SaveAsync(evolution, cancellationToken);

    EvolutionModel model = await _evolutionQuerier.ReadAsync(evolution, cancellationToken);
    return new CreateOrReplaceEvolutionResult(model, created);
  }
}
