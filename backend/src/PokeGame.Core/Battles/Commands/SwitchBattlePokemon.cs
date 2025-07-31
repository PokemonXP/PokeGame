using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Battles.Validators;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Commands;

internal record SwitchBattlePokemon(Guid Id, SwitchBattlePokemonPayload Payload) : ICommand<BattleModel?>;

/// <exception cref="PokemonNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class SwitchBattlePokemonHandler : ICommandHandler<SwitchBattlePokemon, BattleModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;
  private readonly IPokemonRepository _pokemonRepository;

  public SwitchBattlePokemonHandler(
    IApplicationContext applicationContext,
    IBattleQuerier battleQuerier,
    IBattleRepository battleRepository,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _battleQuerier = battleQuerier;
    _battleRepository = battleRepository;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<BattleModel?> HandleAsync(SwitchBattlePokemon command, CancellationToken cancellationToken = default)
  {
    ActorId? actorId = _applicationContext.ActorId;

    SwitchBattlePokemonPayload payload = command.Payload;
    new SwitchBattlePokemonValidator().ValidateAndThrow(payload);

    BattleId battleId = new(command.Id);
    Battle? battle = await _battleRepository.LoadAsync(battleId, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    Specimen active = await _pokemonRepository.LoadAsync(payload.Active, cancellationToken)
      ?? throw new PokemonNotFoundException([payload.Active], nameof(payload.Active));
    Specimen inactive = await _pokemonRepository.LoadAsync(payload.Inactive, cancellationToken)
      ?? throw new PokemonNotFoundException([payload.Inactive], nameof(payload.Inactive));

    battle.Switch(active, inactive, actorId);

    await _battleRepository.SaveAsync(battle, cancellationToken);

    return await _battleQuerier.ReadAsync(battle, cancellationToken);
  }
}
