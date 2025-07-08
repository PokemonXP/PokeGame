using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedTechnicalMachinesTask : SeedingTask
{
  public override string? Description => "Seeds Technical Machine contents into Krakenar.";
  public string Language { get; }

  public SeedTechnicalMachinesTask(string language)
  {
    Language = language;
  }
}

internal class SeedTechnicalMachinesTaskHandler : INotificationHandler<SeedTechnicalMachinesTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedTechnicalMachinesTaskHandler> _logger;

  public SeedTechnicalMachinesTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedTechnicalMachinesTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedTechnicalMachinesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<TechnicalMachinePayload> technicalMachines = await CsvHelper.ExtractAsync<TechnicalMachinePayload>("Game/data/items/technical_machines.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Moves.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    ContentIndex moveIndex = new(results);

    search.ContentTypeId = TechnicalMachines.ContentTypeId;
    results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    TechnicalMachineValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (TechnicalMachinePayload technicalMachine in technicalMachines)
    {
      ValidationResult result = validator.Validate(technicalMachine);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The technical machine '{TechnicalMachine}' was not seeded because there are validation errors.|Errors: {Errors}", technicalMachine, errors);
        continue;
      }

      Guid? moveId = moveIndex.Get(technicalMachine.Move)?.Content.Id;
      if (!moveId.HasValue)
      {
        _logger.LogError("The technical machine '{TechnicalMachine}' was not seeded because the move '{Move}' was not found.", technicalMachine, technicalMachine.Move);
        continue;
      }

      string move = SeedingSerializer.Serialize<Guid[]>([moveId.Value]);

      Content content;
      if (existingIds.Contains(technicalMachine.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = technicalMachine.UniqueName,
          DisplayName = technicalMachine.DisplayName,
          Description = technicalMachine.Description
        };
        invariant.FieldValues.Add(TechnicalMachines.Price, technicalMachine.Price);
        invariant.FieldValues.Add(TechnicalMachines.Move, move);
        invariant.FieldValues.Add(TechnicalMachines.Sprite, technicalMachine.Sprite);
        _ = await _contentService.SaveLocaleAsync(technicalMachine.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = technicalMachine.UniqueName,
          DisplayName = technicalMachine.DisplayName,
          Description = technicalMachine.Description
        };
        locale.FieldValues.Add(TechnicalMachines.Url, technicalMachine.Url);
        locale.FieldValues.Add(TechnicalMachines.Notes, technicalMachine.Notes);
        content = await _contentService.SaveLocaleAsync(technicalMachine.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The technical machine content 'Id={technicalMachine.Id}' was not found.");
        _logger.LogInformation("The technical machine content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = technicalMachine.Id,
          ContentType = TechnicalMachines.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = technicalMachine.UniqueName,
          DisplayName = technicalMachine.DisplayName,
          Description = technicalMachine.Description
        };
        payload.FieldValues.Add(TechnicalMachines.Price, technicalMachine.Price);
        payload.FieldValues.Add(TechnicalMachines.Move, move);
        payload.FieldValues.Add(TechnicalMachines.Sprite, technicalMachine.Sprite);
        payload.FieldValues.Add(TechnicalMachines.Url, technicalMachine.Url);
        payload.FieldValues.Add(TechnicalMachines.Notes, technicalMachine.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The technical machine content 'Id={ContentId}' was created.", content.Id);
      }

      await _contentService.PublishAsync(content.Id, language: null, cancellationToken);
      await _contentService.PublishAsync(content.Id, task.Language, cancellationToken);
      _logger.LogInformation("The technical machine content 'Id={ContentId}' was published.", content.Id);
    }
  }
}
