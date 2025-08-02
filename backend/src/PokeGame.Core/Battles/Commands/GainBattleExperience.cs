using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Battles.Validators;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Commands;

internal record GainBattleExperience(Guid Id, GainBattleExperiencePayload Payload) : ICommand<BattleModel?>;

/// <exception cref="PokemonNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class GainBattleExperienceHandler : ICommandHandler<GainBattleExperience, BattleModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleManager _battleManager;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;
  private readonly IMoveRepository _moveRepository;
  private readonly IPokemonRepository _pokemonRepository;

  public GainBattleExperienceHandler(
    IApplicationContext applicationContext,
    IBattleManager battleManager,
    IBattleQuerier battleQuerier,
    IBattleRepository battleRepository,
    IMoveRepository moveRepository,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _battleManager = battleManager;
    _battleQuerier = battleQuerier;
    _battleRepository = battleRepository;
    _moveRepository = moveRepository;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<BattleModel?> HandleAsync(GainBattleExperience command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    GainBattleExperiencePayload payload = command.Payload;
    new GainBattleExperienceValidator().ValidateAndThrow(payload);

    BattleId battleId = new(command.Id);
    Battle? battle = await _battleRepository.LoadAsync(battleId, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    Specimen defeated = await _pokemonRepository.LoadAsync(payload.Defeated, cancellationToken)
      ?? throw new PokemonNotFoundException([payload.Defeated], nameof(payload.Defeated));

    IEnumerable<string> victoriousIds = payload.Victorious.Select(x => x.Pokemon).Distinct();
    IReadOnlyDictionary<string, Specimen> victorious = await _battleManager.FindPokemonAsync(victoriousIds, nameof(payload.Victorious), cancellationToken);
    foreach (VictoriousPokemonPayload battler in payload.Victorious)
    {
      Specimen pokemon = victorious[battler.Pokemon];
      if (pokemon.Equals(defeated))
      {
        throw new NotImplementedException(); // TODO(fpion): implement
      }

      pokemon.Gain(battler.Experience, actorId);
      battle.Gain(defeated, pokemon, battler.Experience, actorId);

      // TODO(fpion): learned moves
    }

    await _battleRepository.SaveAsync(battle, cancellationToken);
    await _pokemonRepository.SaveAsync(victorious.Values, cancellationToken);

    return await _battleQuerier.ReadAsync(battle, cancellationToken);
  }
}
