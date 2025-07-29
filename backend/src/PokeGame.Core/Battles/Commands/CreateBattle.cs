using FluentValidation;
using Krakenar.Core;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Battles.Validators;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Battles.Commands;

internal record CreateBattle(CreateBattlePayload Payload) : ICommand<BattleModel>;

/// <exception cref="ValidationException"></exception>
internal class CreateBattleHandler : ICommandHandler<CreateBattle, BattleModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;

  public CreateBattleHandler(IApplicationContext applicationContext, IBattleQuerier battleQuerier, IBattleRepository battleRepository)
  {
    _applicationContext = applicationContext;
    _battleQuerier = battleQuerier;
    _battleRepository = battleRepository;
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
      await Task.Delay(1000, cancellationToken); // TODO(fpion): implement
    }

    return Battle.Trainer(name, location, champions, opponents, url, notes, _applicationContext.ActorId, battleId);
  }

  private async Task<Battle> CreateWildPokemonAsync(CreateBattlePayload payload, BattleId battleId, CancellationToken cancellationToken)
  {
    DisplayName name = new(payload.Name);
    Location location = new(payload.Location);
    Url? url = Url.TryCreate(payload.Url);
    Notes? notes = Notes.TryCreate(payload.Notes);

    await Task.Delay(1000, cancellationToken); // TODO(fpion): implement
    List<Trainer> champions = []; // TODO(fpion): implement
    List<Specimen> opponents = []; // TODO(fpion): implement

    return Battle.WildPokemon(name, location, champions, opponents, url, notes, _applicationContext.ActorId, battleId);
  }
}
