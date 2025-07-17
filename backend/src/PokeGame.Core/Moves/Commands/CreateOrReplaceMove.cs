using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Moves.Validators;

namespace PokeGame.Core.Moves.Commands;

internal record CreateOrReplaceMove(CreateOrReplaceMovePayload Payload, Guid? Id) : ICommand<CreateOrReplaceMoveResult>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceMoveHandler : ICommandHandler<CreateOrReplaceMove, CreateOrReplaceMoveResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMoveManager _moveManager;
  private readonly IMoveQuerier _moveQuerier;
  private readonly IMoveRepository _moveRepository;

  public CreateOrReplaceMoveHandler(
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

  public async Task<CreateOrReplaceMoveResult> HandleAsync(CreateOrReplaceMove command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreateOrReplaceMovePayload payload = command.Payload;
    new CreateOrReplaceMoveValidator(uniqueNameSettings).ValidateAndThrow(payload);

    MoveId moveId = MoveId.NewId();
    Move? move = null;
    if (command.Id.HasValue)
    {
      moveId = new(command.Id.Value);
      move = await _moveRepository.LoadAsync(moveId, cancellationToken);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    Accuracy? accuracy = payload.Accuracy.HasValue ? new(payload.Accuracy.Value) : null;
    Power? power = payload.Power.HasValue ? new(payload.Power.Value) : null;
    PowerPoints powerPoints = new(payload.PowerPoints);

    bool created = false;
    if (move is null)
    {
      move = new(payload.Type, payload.Category, uniqueName, powerPoints, accuracy, power, actorId, moveId);
      created = true;
    }
    else
    {
      move.SetUniqueName(uniqueName, actorId);

      move.Accuracy = accuracy;
      move.Power = power;
      move.PowerPoints = powerPoints;
    }

    move.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    move.Description = Description.TryCreate(payload.Description);

    move.Url = Url.TryCreate(payload.Url);
    move.Notes = Notes.TryCreate(payload.Notes);

    move.Update(actorId);
    await _moveManager.SaveAsync(move, cancellationToken);

    MoveModel model = await _moveQuerier.ReadAsync(move, cancellationToken);
    return new CreateOrReplaceMoveResult(model, created);
  }
}
