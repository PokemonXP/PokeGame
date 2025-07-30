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

    Specimen source = await _pokemonRepository.LoadAsync(payload.Source, cancellationToken)
      ?? throw new PokemonNotFoundException([payload.Source], nameof(payload.Source));
    Specimen destination = await _pokemonRepository.LoadAsync(payload.Destination, cancellationToken)
      ?? throw new PokemonNotFoundException([payload.Destination], nameof(payload.Destination));

    Specimen[] specimens = [source, destination];
    List<ValidationFailure> failures = new(capacity: specimens.Length + 1);
    foreach (Specimen pokemon in specimens)
    {
      if (pokemon.Ownership is null || pokemon.Slot is null)
      {
        ValidationFailure failure = new("PokemonId", "The Pokémon is not owned by any trainer.", pokemon.Id.ToGuid())
        {
          ErrorCode = "PokemonHasNoOwner"
        };
        failures.Add(failure);
      }
    }
    if (source.Ownership is not null && destination.Ownership is not null && source.Ownership.TrainerId != destination.Ownership.TrainerId)
    {
      Guid[] pokemonIds = [source.Id.ToGuid(), destination.Id.ToGuid()];
      ValidationFailure failure = new("PokemonIds", "The Pokémon are not owned by the same trainer.", pokemonIds)
      {
        CustomState = new
        {
          SourceTrainerId = source.Ownership.TrainerId.ToGuid(),
          DestinationTrainerId = destination.Ownership.TrainerId.ToGuid()
        },
        ErrorCode = "PokemonOwnersAreDifferent"
      };
      failures.Add(failure);
    }
    if (failures.Count > 0)
    {
      throw new ValidationException(failures);
    }

    if ((IsEggInBox(source) && IsHatchedInParty(destination)) || (IsEggInBox(destination) && IsHatchedInParty(source)))
    {
      TrainerId trainerId = source.Ownership?.TrainerId ?? new();
      Storage storage = await _pokemonQuerier.GetStorageAsync(trainerId, cancellationToken);
      IEnumerable<PokemonId> partyIds = storage.Party.Except([source.Id, destination.Id]);
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

    source.Swap(destination, actorId);
    await _pokemonRepository.SaveAsync([source, destination], cancellationToken);

    SearchPokemonPayload search = new();
    search.Ids.Add(source.Id.ToGuid());
    search.Ids.Add(destination.Id.ToGuid());
    SearchResults<PokemonModel> results = await _pokemonQuerier.SearchAsync(search, cancellationToken);
    return results.Items­.AsReadOnly();
  }

  private static bool IsEggInBox(Specimen pokemon) => pokemon.IsEgg && pokemon.Slot is not null && pokemon.Slot.Box is not null;
  private static bool IsHatchedInParty(Specimen pokemon) => !pokemon.IsEgg && pokemon.Slot is not null && pokemon.Slot.Box is null;
}
