using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Storage;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Commands;

internal record ReleasePokemon(Guid Id) : ICommand<PokemonModel?>;

/// <exception cref="ValidationException"></exception>
internal class ReleasePokemonHandler : ICommandHandler<ReleasePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly ITrainerRepository _trainerRepository;

  public ReleasePokemonHandler(
    IApplicationContext applicationContext,
    IPokemonManager pokemonManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository,
    ITrainerRepository trainerRepository)
  {
    _applicationContext = applicationContext;
    _pokemonManager = pokemonManager;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
    _trainerRepository = trainerRepository;
  }

  public async Task<PokemonModel?> HandleAsync(ReleasePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    if (pokemon.Ownership is null || pokemon.Slot is null)
    {
      ValidationFailure failure = new(nameof(command.Id), "The Pokémon is not owned by any trainer.", pokemon.Id.ToGuid())
      {
        ErrorCode = "PokemonHasNoOwner"
      };
      throw new ValidationException([failure]);
    }

    PokemonStorage storage = await _trainerRepository.LoadStorageAsync(pokemon.Ownership.TrainerId, cancellationToken);
    IEnumerable<PokemonId> partyIds = storage.GetParty().Except([pokemon.Id]);
    Dictionary<PokemonId, Specimen> party = (await _pokemonRepository.LoadAsync(partyIds, cancellationToken)).ToDictionary(x => x.Id, x => x);
    storage.Release(pokemon, party, actorId);

    await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    await _trainerRepository.SaveAsync(storage, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
