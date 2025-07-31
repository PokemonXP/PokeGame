using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Storage;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Commands;

internal record DepositPokemon(Guid Id) : ICommand<PokemonModel?>;

/// <exception cref="ValidationException"></exception>
internal class DepositPokemonHandler : ICommandHandler<DepositPokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly ITrainerRepository _trainerRepository;

  public DepositPokemonHandler(
    IApplicationContext applicationContext,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository,
    ITrainerRepository trainerRepository)
  {
    _applicationContext = applicationContext;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
    _trainerRepository = trainerRepository;
  }

  public async Task<PokemonModel?> HandleAsync(DepositPokemon command, CancellationToken cancellationToken)
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
    else if (pokemon.Slot.Box is not null)
    {
      ValidationFailure failure = new(nameof(command.Id), "The Pokémon is already in a box.", pokemon.Id.ToGuid())
      {
        ErrorCode = "PokemonIsAlreadyInABox"
      };
      throw new ValidationException([failure]);
    }

    PokemonStorage storage = await _trainerRepository.LoadStorageAsync(pokemon.Ownership.TrainerId, cancellationToken);
    IEnumerable<PokemonId> partyIds = storage.GetParty().Except([pokemon.Id]);
    Dictionary<PokemonId, Specimen> party = (await _pokemonRepository.LoadAsync(partyIds, cancellationToken)).ToDictionary(x => x.Id, x => x);
    storage.Deposit(pokemon, party.AsReadOnly(), actorId);

    await _pokemonRepository.SaveAsync(new[] { pokemon }.Concat(party.Values), cancellationToken);

    await _trainerRepository.SaveAsync(storage, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
