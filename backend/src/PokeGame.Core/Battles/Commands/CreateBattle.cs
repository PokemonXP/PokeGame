using FluentValidation;
using Krakenar.Core;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Battles.Validators;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Battles.Commands;

internal record CreateBattle(CreateBattlePayload Payload) : ICommand<BattleModel>;

/// <exception cref="IdAlreadyUsedException{T}"></exception>
/// <exception cref="PokemonNotFoundException"></exception>
/// <exception cref="TrainersNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateBattleHandler : ICommandHandler<CreateBattle, BattleModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly ITrainerQuerier _trainerQuerier;
  private readonly ITrainerRepository _trainerRepository;

  public CreateBattleHandler(
    IApplicationContext applicationContext,
    IBattleQuerier battleQuerier,
    IBattleRepository battleRepository,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository,
    ITrainerQuerier trainerQuerier,
    ITrainerRepository trainerRepository)
  {
    _applicationContext = applicationContext;
    _battleQuerier = battleQuerier;
    _battleRepository = battleRepository;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
    _trainerQuerier = trainerQuerier;
    _trainerRepository = trainerRepository;
  }

  public async Task<BattleModel> HandleAsync(CreateBattle command, CancellationToken cancellationToken)
  {
    CreateBattlePayload payload = command.Payload;
    new CreateBattleValidator().ValidateAndThrow(payload);

    BattleId battleId = BattleId.NewId();
    Battle? battle;
    if (payload.Id.HasValue)
    {
      battleId = new(payload.Id.Value);
      battle = await _battleRepository.LoadAsync(battleId, cancellationToken);
      if (battle is not null)
      {
        throw new IdAlreadyUsedException<Battle>(_applicationContext.RealmId, payload.Id.Value, nameof(payload.Id));
      }
    }

    battle = payload.Kind switch
    {
      BattleKind.Trainer => await CreateTrainerAsync(payload, battleId, cancellationToken),
      BattleKind.WildPokemon => await CreateWildPokemonAsync(payload, battleId, cancellationToken),
      _ => throw new NotSupportedException($"The battle kind '{payload.Kind}' is not supported."),
    };

    await _battleRepository.SaveAsync(battle, cancellationToken);

    return await _battleQuerier.ReadAsync(battle, cancellationToken);
  }

  private async Task<Battle> CreateTrainerAsync(CreateBattlePayload payload, BattleId battleId, CancellationToken cancellationToken)
  {
    DisplayName name = new(payload.Name);
    Location location = new(payload.Location);
    Url? url = Url.TryCreate(payload.Url);
    Notes? notes = Notes.TryCreate(payload.Notes);

    IEnumerable<string> trainerIds = payload.Champions.Concat(payload.Opponents).Distinct();
    int capacity = trainerIds.Count();
    List<Trainer> champions = new(capacity);
    List<Trainer> opponents = new(capacity);
    if (capacity > 0)
    {
      IReadOnlyDictionary<string, Trainer> trainers = await LoadTrainersAsync(trainerIds, cancellationToken);
      foreach (string champion in payload.Champions)
      {
        champions.Add(trainers[champion]);
      }
      foreach (string opponent in payload.Opponents)
      {
        champions.Add(trainers[opponent]);
      }
    }

    return Battle.Trainer(name, location, champions, opponents, url, notes, _applicationContext.ActorId, battleId);
  }

  private async Task<Battle> CreateWildPokemonAsync(CreateBattlePayload payload, BattleId battleId, CancellationToken cancellationToken)
  {
    DisplayName name = new(payload.Name);
    Location location = new(payload.Location);
    Url? url = Url.TryCreate(payload.Url);
    Notes? notes = Notes.TryCreate(payload.Notes);

    IReadOnlyCollection<Trainer> champions = (await LoadTrainersAsync(payload.Champions, nameof(payload.Champions), cancellationToken)).Values.ToList().AsReadOnly();
    IReadOnlyCollection<Specimen> opponents = (await LoadPokemonAsync(payload.Opponents, nameof(payload.Opponents), cancellationToken)).Values.ToList().AsReadOnly();

    return Battle.WildPokemon(name, location, champions, opponents, url, notes, _applicationContext.ActorId, battleId);
  }

  private async Task<IReadOnlyDictionary<string, Specimen>> LoadPokemonAsync(IEnumerable<string> idOrUniqueNames, string propertyName, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<PokemonKey> keys = await _pokemonQuerier.GetKeysAsync(cancellationToken);
    Dictionary<Guid, PokemonId> pokemonByIds = new(capacity: keys.Count);
    Dictionary<string, PokemonId> pokemonByNames = new(capacity: keys.Count);
    foreach (PokemonKey key in keys)
    {
      pokemonByIds[key.Id] = key.PokemonId;
      pokemonByNames[Normalize(key.UniqueName)] = key.PokemonId;
    }

    int capacity = idOrUniqueNames.Count();
    Dictionary<string, PokemonId> foundPokemon = new(capacity);
    HashSet<string> missingPokemon = new(capacity);
    foreach (string idOrUniqueName in idOrUniqueNames)
    {
      if ((Guid.TryParse(idOrUniqueName, out Guid id) && pokemonByIds.TryGetValue(id, out PokemonId pokemonId))
        || pokemonByNames.TryGetValue(Normalize(idOrUniqueName), out pokemonId))
      {
        foundPokemon[idOrUniqueName] = pokemonId;
      }
      else
      {
        missingPokemon.Add(idOrUniqueName);
      }
    }
    if (missingPokemon.Count > 0)
    {
      throw new PokemonNotFoundException(missingPokemon, propertyName);
    }

    Dictionary<PokemonId, Specimen> pokemon = (await _pokemonRepository.LoadAsync(foundPokemon.Values, cancellationToken)).ToDictionary(x => x.Id, x => x);
    return foundPokemon.ToDictionary(x => x.Key, x => pokemon[x.Value]).AsReadOnly();
  }

  private async Task<IReadOnlyDictionary<string, Trainer>> LoadTrainersAsync(IEnumerable<string> idOrUniqueNames, CancellationToken cancellationToken)
  {
    return await LoadTrainersAsync(idOrUniqueNames, propertyName: null, cancellationToken);
  }
  private async Task<IReadOnlyDictionary<string, Trainer>> LoadTrainersAsync(IEnumerable<string> idOrUniqueNames, string? propertyName, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<TrainerKey> keys = await _trainerQuerier.GetKeysAsync(cancellationToken);
    Dictionary<Guid, TrainerId> trainerByIds = new(capacity: keys.Count);
    Dictionary<string, TrainerId> trainerByNames = new(capacity: keys.Count);
    foreach (TrainerKey key in keys)
    {
      trainerByIds[key.Id] = key.TrainerId;
      trainerByNames[Normalize(key.UniqueName)] = key.TrainerId;
    }

    int capacity = idOrUniqueNames.Count();
    Dictionary<string, TrainerId> foundTrainers = new(capacity);
    HashSet<string> missingTrainers = new(capacity);
    foreach (string idOrUniqueName in idOrUniqueNames)
    {
      if (!string.IsNullOrWhiteSpace(idOrUniqueName))
      {
        if ((Guid.TryParse(idOrUniqueName, out Guid id) && trainerByIds.TryGetValue(id, out TrainerId trainerId))
          || trainerByNames.TryGetValue(Normalize(idOrUniqueName), out trainerId))
        {
          foundTrainers[idOrUniqueName] = trainerId;
        }
        else
        {
          missingTrainers.Add(idOrUniqueName);
        }
      }
    }
    if (missingTrainers.Count > 0)
    {
      throw new TrainersNotFoundException(missingTrainers, propertyName);
    }

    Dictionary<TrainerId, Trainer> trainers = (await _trainerRepository.LoadAsync(foundTrainers.Values, cancellationToken)).ToDictionary(x => x.Id, x => x);
    return foundTrainers.ToDictionary(x => x.Key, x => trainers[x.Value]).AsReadOnly();
  }

  private static string Normalize(string value) => value.Trim().ToLowerInvariant();
}
