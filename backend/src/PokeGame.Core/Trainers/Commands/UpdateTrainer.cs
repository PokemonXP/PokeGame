using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Trainers.Models;
using PokeGame.Core.Trainers.Validators;

namespace PokeGame.Core.Trainers.Commands;

internal record UpdateTrainer(Guid Id, UpdateTrainerPayload Payload) : ICommand<TrainerModel?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateTrainerHandler : ICommandHandler<UpdateTrainer, TrainerModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITrainerManager _trainerManager;
  private readonly ITrainerQuerier _trainerQuerier;
  private readonly ITrainerRepository _trainerRepository;

  public UpdateTrainerHandler(
    IApplicationContext applicationContext,
    ITrainerManager trainerManager,
    ITrainerQuerier trainerQuerier,
    ITrainerRepository trainerRepository)
  {
    _applicationContext = applicationContext;
    _trainerManager = trainerManager;
    _trainerQuerier = trainerQuerier;
    _trainerRepository = trainerRepository;
  }

  public async Task<TrainerModel?> HandleAsync(UpdateTrainer command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdateTrainerPayload payload = command.Payload;
    new UpdateTrainerValidator(uniqueNameSettings).ValidateAndThrow(payload);

    TrainerId trainerId = new(command.Id);
    Trainer? trainer = await _trainerRepository.LoadAsync(trainerId, cancellationToken);
    if (trainer is null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      trainer.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      trainer.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      trainer.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Gender.HasValue)
    {
      trainer.Gender = payload.Gender.Value;
    }
    if (payload.Money.HasValue)
    {
      trainer.Money = new Money(payload.Money.Value);
    }

    if (payload.UserId is not null)
    {
      trainer.UserId = payload.UserId.Value;
    }

    if (payload.Sprite is not null)
    {
      trainer.Sprite = Url.TryCreate(payload.Sprite.Value);
    }
    if (payload.Url is not null)
    {
      trainer.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      trainer.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    trainer.Update(_applicationContext.ActorId);
    await _trainerManager.SaveAsync(trainer, cancellationToken);

    return await _trainerQuerier.ReadAsync(trainer, cancellationToken);
  }
}
