using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Evolutions.Models;
using PokeGame.Core.Evolutions.Validators;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;

namespace PokeGame.Core.Evolutions.Commands;

internal record UpdateEvolution(Guid Id, UpdateEvolutionPayload Payload) : ICommand<EvolutionModel?>;

/// <exception cref="ItemNotFoundException"></exception>
/// <exception cref="MoveNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateEvolutionHandler : ICommandHandler<UpdateEvolution, EvolutionModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IEvolutionQuerier _evolutionQuerier;
  private readonly IEvolutionRepository _evolutionRepository;
  private readonly IItemRepository _itemRepository;
  private readonly IMoveRepository _moveRepository;

  public UpdateEvolutionHandler(
    IApplicationContext applicationContext,
    IEvolutionQuerier evolutionQuerier,
    IEvolutionRepository evolutionRepository,
    IItemRepository itemRepository,
    IMoveRepository moveRepository)
  {
    _applicationContext = applicationContext;
    _evolutionQuerier = evolutionQuerier;
    _evolutionRepository = evolutionRepository;
    _itemRepository = itemRepository;
    _moveRepository = moveRepository;
  }

  public async Task<EvolutionModel?> HandleAsync(UpdateEvolution command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    UpdateEvolutionPayload payload = command.Payload;
    new UpdateEvolutionValidator().ValidateAndThrow(payload);

    EvolutionId evolutionId = new(command.Id);
    Evolution? evolution = await _evolutionRepository.LoadAsync(evolutionId, cancellationToken);
    if (evolution is null)
    {
      return null;
    }

    if (payload.Level.HasValue)
    {
      evolution.Level = payload.Level < 1 ? null : new Level(payload.Level.Value);
    }
    if (payload.Friendship.HasValue)
    {
      evolution.Friendship = payload.Friendship.Value;
    }
    if (payload.Gender is not null)
    {
      evolution.Gender = payload.Gender.Value;
    }
    if (payload.HeldItem is not null)
    {
      Item? heldItem = string.IsNullOrWhiteSpace(payload.HeldItem.Value) ? null
        : (await _itemRepository.LoadAsync(payload.HeldItem.Value, cancellationToken) ?? throw new ItemNotFoundException(payload.HeldItem.Value, nameof(payload.HeldItem)));
      evolution.HeldItemId = heldItem?.Id;
    }
    if (payload.KnownMove is not null)
    {
      Move? knownMove = string.IsNullOrWhiteSpace(payload.KnownMove.Value) ? null
        : (await _moveRepository.LoadAsync(payload.KnownMove.Value, cancellationToken) ?? throw new MoveNotFoundException(payload.KnownMove.Value, nameof(payload.KnownMove)));
      evolution.KnownMoveId = knownMove?.Id;
    }
    if (payload.Location is not null)
    {
      evolution.Location = string.IsNullOrWhiteSpace(payload.Location.Value) ? null : new Location(payload.Location.Value);
    }
    if (payload.TimeOfDay is not null)
    {
      evolution.TimeOfDay = payload.TimeOfDay.Value;
    }

    evolution.Update(actorId);
    await _evolutionRepository.SaveAsync(evolution, cancellationToken);

    return await _evolutionQuerier.ReadAsync(evolution, cancellationToken);
  }
}
