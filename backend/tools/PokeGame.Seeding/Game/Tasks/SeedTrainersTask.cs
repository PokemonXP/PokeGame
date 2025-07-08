using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Core;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedTrainersTask : SeedingTask
{
  public override string? Description => "Seeds Trainer contents into Krakenar.";
  public string Language { get; }

  public SeedTrainersTask(string language)
  {
    Language = language;
  }
}

internal class SeedTrainersTaskHandler : INotificationHandler<SeedTrainersTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedTrainersTaskHandler> _logger;

  public SeedTrainersTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedTrainersTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedTrainersTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<TrainerPayload> trainers = await CsvHelper.ExtractAsync<TrainerPayload>("Game/data/trainers.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Trainers.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    TrainerValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (TrainerPayload trainer in trainers)
    {
      ValidationResult result = validator.Validate(trainer);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The trainer '{Trainer}' was not seeded because there are validation errors.|Errors: {Errors}", trainer, errors);
        continue;
      }

      string gender = SeedingSerializer.Serialize<string[]>([PokemonConverter.Instance.FromTrainerGender(trainer.Gender)]);

      Content content;
      if (existingIds.Contains(trainer.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = trainer.UniqueName,
          DisplayName = trainer.DisplayName,
          Description = trainer.Description
        };
        invariant.FieldValues.Add(Trainers.Gender, gender);
        invariant.FieldValues.Add(Trainers.License, trainer.License);
        invariant.FieldValues.Add(Trainers.Money, trainer.Money);
        invariant.FieldValues.Add(Trainers.Sprite, trainer.Sprite);
        invariant.FieldValues.Add(Trainers.UserId, trainer.UserId);
        _ = await _contentService.SaveLocaleAsync(trainer.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = trainer.UniqueName,
          DisplayName = trainer.DisplayName,
          Description = trainer.Description
        };
        locale.FieldValues.Add(Trainers.Url, trainer.Url);
        locale.FieldValues.Add(Trainers.Notes, trainer.Notes);
        content = await _contentService.SaveLocaleAsync(trainer.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The trainer content 'Id={trainer.Id}' was not found.");
        _logger.LogInformation("The trainer content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = trainer.Id,
          ContentType = Trainers.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = trainer.UniqueName,
          DisplayName = trainer.DisplayName,
          Description = trainer.Description
        };
        payload.FieldValues.Add(Trainers.Gender, gender);
        payload.FieldValues.Add(Trainers.License, trainer.License);
        payload.FieldValues.Add(Trainers.Money, trainer.Money);
        payload.FieldValues.Add(Trainers.Sprite, trainer.Sprite);
        payload.FieldValues.Add(Trainers.UserId, trainer.UserId);
        payload.FieldValues.Add(Trainers.Url, trainer.Url);
        payload.FieldValues.Add(Trainers.Notes, trainer.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The trainer content 'Id={ContentId}' was created.", content.Id);
      }

      await _contentService.PublishAsync(content.Id, language: null, cancellationToken);
      await _contentService.PublishAsync(content.Id, task.Language, cancellationToken);
      _logger.LogInformation("The trainer content 'Id={ContentId}' was published.", content.Id);
    }
  }
}
