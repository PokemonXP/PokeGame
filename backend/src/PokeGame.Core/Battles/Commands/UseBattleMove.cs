using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Battles.Validators;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Commands;

internal record UseBattleMove(Guid Id, UseBattleMovePayload Payload) : ICommand<BattleModel?>;

/// <exception cref="MoveNotFoundException"></exception>
/// <exception cref="PokemonNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UseBattleMoveHandler : ICommandHandler<UseBattleMove, BattleModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleManager _battleManager;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;
  private readonly IMoveRepository _moveRepository;
  private readonly IPokemonRepository _pokemonRepository;

  public UseBattleMoveHandler(
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

  public async Task<BattleModel?> HandleAsync(UseBattleMove command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    UseBattleMovePayload payload = command.Payload;
    new UseBattleMoveValidator().ValidateAndThrow(payload);

    BattleId battleId = new(command.Id);
    Battle? battle = await _battleRepository.LoadAsync(battleId, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    Specimen attacker = await _pokemonRepository.LoadAsync(payload.Attacker, cancellationToken) ?? throw new PokemonNotFoundException([payload.Attacker], nameof(payload.Attacker));
    IReadOnlyDictionary<string, Specimen> targets = await _battleManager.FindPokemonAsync(payload.Targets.Select(x => x.Target), nameof(payload.Targets), cancellationToken);

    Move move = await _moveRepository.LoadAsync(payload.Move, cancellationToken) ?? throw new MoveNotFoundException(payload.Move, nameof(payload.Move));
    PowerPoints? powerPointCost = payload.PowerPointCost < 1 ? null : new(payload.PowerPointCost);
    attacker.UseMove(move, powerPointCost, payload.StaminaCost, actorId);

    foreach (BattleMoveTargetPayload target in payload.Targets)
    {
      Specimen pokemon = targets[target.Target];

      int? damage = null;
      int? healing = null;
      StatusCondition? inflictedCondition = null;
      StatusCondition? removedCondition = null;
      bool removeAllConditions = false;
      if (target.Damage is not null)
      {
        int value = target.Damage.IsPercentage ? (pokemon.Statistics.HP * target.Damage.Value / 100) : target.Damage.Value;
        if (target.Damage.IsHealing)
        {
          healing = value;
        }
        else
        {
          damage = value;
        }
      }
      if (target.Status is not null)
      {
        if (target.Status.AllConditions)
        {
          removeAllConditions = true;
        }
        else if (target.Status.RemoveCondition)
        {
          removedCondition = target.Status.Condition;
        }
        else
        {
          inflictedCondition = target.Status.Condition;
        }
      }

      if (damage.HasValue || inflictedCondition.HasValue)
      {
        pokemon.Wound(damage, inflictedCondition, actorId);
      }
      if (healing.HasValue || removedCondition.HasValue || removeAllConditions)
      {
        if (removeAllConditions)
        {
          pokemon.Heal(healing, removeAllConditions, actorId);
        }
        else
        {
          pokemon.Heal(healing, removedCondition, actorId);
        }
      }

      // TODO(fpion): apply Statistics
    }

    await _pokemonRepository.SaveAsync(new Specimen[] { attacker }.Concat(targets.Values), cancellationToken);
    await _battleRepository.SaveAsync(battle, cancellationToken);

    return await _battleQuerier.ReadAsync(battle, cancellationToken);
  }
}
