using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Commands;

internal record ResetBattle(Guid Id) : ICommand<BattleModel?>;

/// <exception cref="ValidationException"></exception>
internal class ResetBattleHandler : ICommandHandler<ResetBattle, BattleModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;
  private readonly IPokemonRepository _pokemonRepository;

  public ResetBattleHandler(
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

  public async Task<BattleModel?> HandleAsync(ResetBattle command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    BattleId battleId = new(command.Id);
    Battle? battle = await _battleRepository.LoadAsync(battleId, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    IReadOnlyDictionary<PokemonId, Specimen> pokemon = (await _pokemonRepository.LoadAsync(battle.Pokemon.Keys, cancellationToken))
      .ToDictionary(x => x.Id, x => x);
    battle.Reset(pokemon, actorId);

    await _battleRepository.SaveAsync(battle, cancellationToken);

    return await _battleQuerier.ReadAsync(battle, cancellationToken);
  }
}
