using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Moves.Validators;

namespace PokeGame.Core.Moves.Commands;

internal record UpdateMove(Guid Id, UpdateMovePayload Payload) : ICommand<MoveModel?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateMoveHandler : ICommandHandler<UpdateMove, MoveModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMoveManager _moveManager;
  private readonly IMoveQuerier _moveQuerier;
  private readonly IMoveRepository _moveRepository;

  public UpdateMoveHandler(
    IApplicationContext applicationContext,
    IMoveManager moveManager,
    IMoveQuerier moveQuerier,
    IMoveRepository moveRepository)
  {
    _applicationContext = applicationContext;
    _moveManager = moveManager;
    _moveQuerier = moveQuerier;
    _moveRepository = moveRepository;
  }

  public async Task<MoveModel?> HandleAsync(UpdateMove command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdateMovePayload payload = command.Payload;
    new UpdateMoveValidator(uniqueNameSettings).ValidateAndThrow(payload);

    MoveId moveId = new(command.Id);
    Move? move = await _moveRepository.LoadAsync(moveId, cancellationToken);
    if (move is null)
    {
      return null;
    }
    new UpdateMoveValidator(move).ValidateAndThrow(payload);

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      move.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      move.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      move.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Accuracy is not null)
    {
      move.Accuracy = payload.Accuracy.Value.HasValue ? new Accuracy(payload.Accuracy.Value.Value) : null;
    }
    if (payload.Power is not null)
    {
      move.Power = payload.Power.Value.HasValue ? new Power(payload.Power.Value.Value) : null;
    }
    if (payload.PowerPoints.HasValue)
    {
      move.PowerPoints = new PowerPoints(payload.PowerPoints.Value);
    }

    if (payload.Url is not null)
    {
      move.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      move.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    move.Update(_applicationContext.ActorId);
    await _moveManager.SaveAsync(move, cancellationToken);

    return await _moveQuerier.ReadAsync(move, cancellationToken);
  }
}
