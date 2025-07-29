using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Inventory;
using PokeGame.Core.Items;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Commands;

internal record ChangePokemonItem(Guid Id, ChangePokemonItemPayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="InsufficientInventoryQuantityException"></exception>
/// <exception cref="ItemNotFoundException"></exception>
/// <exception cref="PokemonHasNoOwnerException"></exception>
internal class ChangePokemonItemHandler : ICommandHandler<ChangePokemonItem, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IItemRepository _itemRepository;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly ITrainerRepository _trainerRepository;

  public ChangePokemonItemHandler(
    IApplicationContext applicationContext,
    IItemRepository itemRepository,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository,
    ITrainerRepository trainerRepository)
  {
    _applicationContext = applicationContext;
    _itemRepository = itemRepository;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
    _trainerRepository = trainerRepository;
  }

  public async Task<PokemonModel?> HandleAsync(ChangePokemonItem command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    ChangePokemonItemPayload payload = command.Payload;

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }
    else if (pokemon.Ownership is null)
    {
      throw new PokemonHasNoOwnerException(pokemon);
    }

    Item? item = null;
    if (!string.IsNullOrWhiteSpace(payload.HeldItem))
    {
      item = await _itemRepository.LoadAsync(payload.HeldItem, cancellationToken) ?? throw new ItemNotFoundException(payload.HeldItem, nameof(payload.HeldItem));
    }

    TrainerInventory inventory = await _trainerRepository.LoadInventoryAsync(pokemon.Ownership.TrainerId, cancellationToken);

    if (item is null)
    {
      if (pokemon.HeldItemId.HasValue)
      {
        inventory.Add(pokemon.HeldItemId.Value, quantity: 1, actorId);
        pokemon.RemoveItem(actorId);
      }
    }
    else
    {
      inventory.EnsureQuantity(item, quantity: 1);

      if (pokemon.HeldItemId.HasValue)
      {
        inventory.Add(pokemon.HeldItemId.Value, quantity: 1, actorId);
      }

      inventory.Remove(item, quantity: 1, actorId);
      pokemon.HoldItem(item, actorId);
    }

    await _pokemonRepository.SaveAsync(pokemon, cancellationToken);
    await _trainerRepository.SaveAsync(inventory, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
