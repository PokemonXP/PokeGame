using FluentValidation;
using FluentValidation.Results;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemon.Validators;
using PokeGame.Core.Storage;
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
  private readonly ITrainerRepository _trainerRepository;

  public SwapPokemonHandler(
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

    if (source.Ownership is null)
    {
      throw new InvalidOperationException("The Pokémon ownership are required.");
    }
    PokemonStorage storage = await _trainerRepository.LoadStorageAsync(source.Ownership.TrainerId, cancellationToken);
    Dictionary<PokemonId, Specimen> party = [];
    if ((source.IsEggInBox && destination.IsHatchedInParty) || (destination.IsEggInBox && source.IsHatchedInParty))
    {
      IEnumerable<PokemonId> partyIds = storage.GetParty().Except([source.Id, destination.Id]);
      party = (await _pokemonRepository.LoadAsync(partyIds, cancellationToken)).ToDictionary(x => x.Id, x => x);
    }
    storage.Swap(source, destination, party.AsReadOnly(), actorId);

    await _pokemonRepository.SaveAsync([source, destination], cancellationToken);

    await _trainerRepository.SaveAsync(storage, cancellationToken);

    SearchPokemonPayload search = new();
    search.Ids.Add(source.Id.ToGuid());
    search.Ids.Add(destination.Id.ToGuid());
    SearchResults<PokemonModel> results = await _pokemonQuerier.SearchAsync(search, cancellationToken);
    return results.Items­.AsReadOnly();
  }
}
