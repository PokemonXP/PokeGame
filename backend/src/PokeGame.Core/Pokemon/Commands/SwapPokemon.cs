using FluentValidation;
using FluentValidation.Results;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemon.Validators;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Commands;

internal record SwapPokemon(SwapPokemonPayload Payload) : ICommand<IReadOnlyCollection<PokemonModel>>;

/// <exception cref="PokemonNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class SwapPokemonHandler : ICommandHandler<SwapPokemon, IReadOnlyCollection<PokemonModel>>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public SwapPokemonHandler(IApplicationContext applicationContext, IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<IReadOnlyCollection<PokemonModel>> HandleAsync(SwapPokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    SwapPokemonPayload payload = command.Payload;
    new SwapPokemonValidator().ValidateAndThrow(payload);

    IEnumerable<PokemonId> pokemonIds = payload.Ids.Select(id => new PokemonId(id));
    IReadOnlyCollection<Specimen> specimens = await _pokemonRepository.LoadAsync(pokemonIds, cancellationToken);

    IEnumerable<Guid> missingIds = payload.Ids.Except(specimens.Select(pokemon => pokemon.Id.ToGuid())).Distinct();
    if (missingIds.Any())
    {
      throw new PokemonNotFoundException(missingIds, nameof(payload.Ids));
    }

    List<ValidationFailure> failures = new(capacity: specimens.Count + 1);
    foreach (Specimen pokemon in specimens)
    {
      if (pokemon.Ownership is null || pokemon.Slot is null)
      {
        ValidationFailure failure = new(nameof(payload.Ids), "The Pokémon is not owned by any trainer.", pokemon.Id.ToGuid())
        {
          ErrorCode = "PokemonHasNoOwner"
        };
        failures.Add(failure);
      }
    }
    Specimen pokemon1 = specimens.First();
    Specimen pokemon2 = specimens.Last();
    if (pokemon1.Ownership is not null && pokemon2.Ownership is not null && pokemon1.Ownership.TrainerId != pokemon2.Ownership.TrainerId)
    {
      ValidationFailure failure = new(nameof(payload.Ids), "The Pokémon are not owned by the same trainer.", payload.Ids)
      {
        ErrorCode = "PokemonOwnersAreDifferent"
      };
      failures.Add(failure);
    }
    if (failures.Count > 0)
    {
      throw new ValidationException(failures);
    }

    if ((IsEggInBox(pokemon1) && IsHatchedInParty(pokemon2)) || (IsEggInBox(pokemon2) && IsHatchedInParty(pokemon1)))
    {
      TrainerId trainerId = pokemon1.Ownership?.TrainerId ?? new();
      Storage storage = await _pokemonQuerier.GetStorageAsync(trainerId, cancellationToken);
      IEnumerable<PokemonId> partyIds = storage.Party.Except([pokemon1.Id, pokemon2.Id]);
      IReadOnlyCollection<Specimen> partyPokemon = await _pokemonRepository.LoadAsync(partyIds, cancellationToken);
      if (!partyPokemon.Any(p => !p.IsEgg))
      {
        ValidationFailure failure = new("TrainerId", "The trainer party must contain at least one other hatched Pokémon.", trainerId.ToGuid())
        {
          ErrorCode = "CannotEmptyTrainerParty"
        };
        throw new ValidationException([failure]);
      }
    }

    pokemon1.Swap(pokemon2, actorId);
    await _pokemonRepository.SaveAsync([pokemon1, pokemon2], cancellationToken);

    SearchPokemonPayload search = new();
    search.Ids.AddRange(payload.Ids);
    SearchResults<PokemonModel> results = await _pokemonQuerier.SearchAsync(search, cancellationToken);
    return results.Items­.AsReadOnly();
  }

  private static bool IsEggInBox(Specimen pokemon) => pokemon.IsEgg && pokemon.Slot is not null && pokemon.Slot.Box is not null;
  private static bool IsHatchedInParty(Specimen pokemon) => !pokemon.IsEgg && pokemon.Slot is not null && pokemon.Slot.Box is null;
}
