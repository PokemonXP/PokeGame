using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using PokeGame.Core.Trainers.Models;
using PokeGame.Core.Trainers.Validators;

namespace PokeGame.Core.Trainers.Commands;

internal record CreateOrReplaceTrainer(CreateOrReplaceTrainerPayload Payload, Guid? Id) : ICommand<CreateOrReplaceTrainerResult>;

/// <exception cref="LicenseAlreadyUsedException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceTrainerHandler : ICommandHandler<CreateOrReplaceTrainer, CreateOrReplaceTrainerResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITrainerManager _trainerManager;
  private readonly ITrainerQuerier _trainerQuerier;
  private readonly ITrainerRepository _trainerRepository;

  public CreateOrReplaceTrainerHandler(
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

  public async Task<CreateOrReplaceTrainerResult> HandleAsync(CreateOrReplaceTrainer command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    RealmId? realmId = _applicationContext.RealmId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreateOrReplaceTrainerPayload payload = command.Payload;
    new CreateOrReplaceTrainerValidator(uniqueNameSettings).ValidateAndThrow(payload);

    TrainerId trainerId = TrainerId.NewId();
    Trainer? trainer = null;
    if (command.Id.HasValue)
    {
      trainerId = new(command.Id.Value);
      trainer = await _trainerRepository.LoadAsync(trainerId, cancellationToken);
    }

    License license = new(payload.License);
    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    Money money = new(payload.Money);

    bool created = false;
    if (trainer is null)
    {
      trainer = new(license, uniqueName, payload.Gender, money, actorId, trainerId);
      created = true;
    }
    else
    {
      trainer.SetUniqueName(uniqueName, actorId);

      trainer.Gender = payload.Gender;
      trainer.Money = money;
    }

    trainer.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    trainer.Description = Description.TryCreate(payload.Description);

    UserId? userId = payload.UserId.HasValue ? new UserId(payload.UserId.Value, realmId) : null;
    trainer.SetUser(userId, actorId);

    trainer.Sprite = Url.TryCreate(payload.Sprite);
    trainer.Url = Url.TryCreate(payload.Url);
    trainer.Notes = Notes.TryCreate(payload.Notes);

    trainer.Update(actorId);
    await _trainerManager.SaveAsync(trainer, cancellationToken);

    TrainerModel model = await _trainerQuerier.ReadAsync(trainer, cancellationToken);
    return new CreateOrReplaceTrainerResult(model, created);
  }
}
