using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Items;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemon.Validators;
using PokeGame.Core.Regions;
using PokeGame.Core.Storage;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Commands;

internal record ReceivePokemon(Guid Id, ReceivePokemonPayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="ItemNotFoundException"></exception>
/// <exception cref="TrainerNotFoundException"></exception>
/// <exception cref="UnexpectedItemCategoryException"></exception>
/// <exception cref="ValidationException"></exception>
internal class ReceivePokemonHandler : ICommandHandler<ReceivePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IItemRepository _itemRepository;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly ITrainerRepository _trainerRepository;

  public ReceivePokemonHandler(
    IApplicationContext applicationContext,
    IItemRepository itemRepository,
    IPokemonManager pokemonManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository,
    ITrainerRepository trainerRepository)
  {
    _applicationContext = applicationContext;
    _itemRepository = itemRepository;
    _pokemonManager = pokemonManager;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
    _trainerRepository = trainerRepository;
  }

  public async Task<PokemonModel?> HandleAsync(ReceivePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    ReceivePokemonPayload payload = command.Payload;
    new ReceivePokemonValidator().ValidateAndThrow(payload);

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    PokemonStorage? previousStorage = pokemon.Ownership is null
      ? null
      : await _trainerRepository.LoadStorageAsync(pokemon.Ownership.TrainerId, cancellationToken);

    Trainer trainer = await _trainerRepository.LoadAsync(payload.Trainer, cancellationToken)
      ?? throw new TrainerNotFoundException(payload.Trainer, nameof(payload.Trainer));
    PokemonStorage storage = await _trainerRepository.LoadStorageAsync(trainer, cancellationToken);

    Item pokeBall = await _itemRepository.LoadAsync(payload.PokeBall, cancellationToken)
      ?? throw new ItemNotFoundException(payload.PokeBall, nameof(payload.PokeBall));
    if (pokeBall.Category != ItemCategory.PokeBall)
    {
      throw new UnexpectedItemCategoryException(ItemCategory.PokeBall, pokeBall, nameof(payload.PokeBall));
    }

    Location location = new(payload.Location);
    Level? level = payload.Level < 1 ? null : new(payload.Level);
    Description? description = Description.TryCreate(payload.Description);
    pokemon.Receive(trainer, pokeBall, location, level, payload.MetOn, description, actorId);

    previousStorage?.Remove(pokemon, actorId);
    storage.Store(pokemon, actorId);

    await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    if (previousStorage is null)
    {
      await _trainerRepository.SaveAsync(storage, cancellationToken);
    }
    else
    {
      await _trainerRepository.SaveAsync([previousStorage, storage], cancellationToken);
    }

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
