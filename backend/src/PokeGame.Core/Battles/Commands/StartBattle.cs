using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Commands;

internal record StartBattle(Guid Id) : ICommand<BattleModel?>;

internal class StartBattleHandler : ICommandHandler<StartBattle, BattleModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;
  private readonly IPokemonRepository _pokemonRepository;

  public StartBattleHandler(
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

  public async Task<BattleModel?> HandleAsync(StartBattle command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    BattleId battleId = new(command.Id);
    Battle? battle = await _battleRepository.LoadAsync(battleId, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    IReadOnlyCollection<Specimen> pokemon = await _pokemonRepository.LoadPartyNonEggsAsync(battle, cancellationToken);
    battle.Start(pokemon, actorId);

    await _battleRepository.SaveAsync(battle, cancellationToken);

    return await _battleQuerier.ReadAsync(battle, cancellationToken);
  }
}
