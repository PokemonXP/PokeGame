using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Battles.Models;
using PokeGame.Core.Battles.Validators;
using PokeGame.Core.Regions;

namespace PokeGame.Core.Battles.Commands;

internal record UpdateBattle(Guid Id, UpdateBattlePayload Payload) : ICommand<BattleModel?>;

/// <exception cref="ValidationException"></exception>
internal class UpdateBattleHandler : ICommandHandler<UpdateBattle, BattleModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IBattleQuerier _battleQuerier;
  private readonly IBattleRepository _battleRepository;

  public UpdateBattleHandler(IApplicationContext applicationContext, IBattleQuerier battleQuerier, IBattleRepository battleRepository)
  {
    _applicationContext = applicationContext;
    _battleQuerier = battleQuerier;
    _battleRepository = battleRepository;
  }

  public async Task<BattleModel?> HandleAsync(UpdateBattle command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    UpdateBattlePayload payload = command.Payload;
    new UpdateBattleValidator().ValidateAndThrow(payload);

    BattleId battleId = new(command.Id);
    Battle? battle = await _battleRepository.LoadAsync(battleId, cancellationToken);
    if (battle is null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      battle.Name = new DisplayName(payload.Name);
    }
    if (!string.IsNullOrWhiteSpace(payload.Location))
    {
      battle.Location = new Location(payload.Location);
    }
    if (payload.Url is not null)
    {
      battle.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      battle.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    battle.Update(actorId);
    await _battleRepository.SaveAsync(battle, cancellationToken);

    return await _battleQuerier.ReadAsync(battle, cancellationToken);
  }
}
