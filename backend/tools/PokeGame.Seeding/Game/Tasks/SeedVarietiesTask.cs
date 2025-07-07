using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedVarietiesTask : SeedingTask
{
  public override string? Description => "Seeds Variety contents into Krakenar.";
  public string Language { get; }

  public SeedVarietiesTask(string language)
  {
    Language = language;
  }
}

internal class SeedVarietiesTaskHandler : INotificationHandler<SeedVarietiesTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedVarietiesTaskHandler> _logger;

  public SeedVarietiesTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedVarietiesTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedVarietiesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<VarietyPayload> varieties = await CsvHelper.ExtractAsync<VarietyPayload>("Game/data/varieties.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Species.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    ContentIndex speciesIndex = new(results);

    search.ContentTypeId = Varieties.ContentTypeId;
    results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    VarietyValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (VarietyPayload variety in varieties)
    {
      ValidationResult result = validator.Validate(variety);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The variety '{Variety}' was not seeded because there are validation errors.|Errors: {Errors}", variety, errors);
        continue;
      }

      Guid? speciesId = speciesIndex.Get(variety.Species)?.Content.Id;
      if (!speciesId.HasValue)
      {
        _logger.LogError("The variety '{Variety}' was not seeded because the species '{Species}' was not found.", variety, variety.Species);
        continue;
      }

      string species = SeedingSerializer.Serialize<Guid[]>([speciesId.Value]);
      string genderRatio = variety.GenderRatio.HasValue ? variety.GenderRatio.Value.ToString() : string.Empty;

      Content content;
      if (existingIds.Contains(variety.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = variety.UniqueName,
          DisplayName = variety.DisplayName,
          Description = variety.Description
        };
        invariant.FieldValues.Add(Varieties.Species, species);
        invariant.FieldValues.Add(Varieties.IsDefault, variety.IsDefault);
        invariant.FieldValues.Add(Varieties.CanChangeForm, variety.CanChangeForm);
        invariant.FieldValues.Add(Varieties.GenderRatio, genderRatio);
        _ = await _contentService.SaveLocaleAsync(variety.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = variety.UniqueName,
          DisplayName = variety.DisplayName,
          Description = variety.Description
        };
        locale.FieldValues.Add(Varieties.Genus, variety.Genus);
        locale.FieldValues.Add(Varieties.Url, variety.Url);
        locale.FieldValues.Add(Varieties.Notes, variety.Notes);
        content = await _contentService.SaveLocaleAsync(variety.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The variety content 'Id={variety.Id}' was not found.");
        _logger.LogInformation("The variety content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = variety.Id,
          ContentType = Varieties.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = variety.UniqueName,
          DisplayName = variety.DisplayName,
          Description = variety.Description
        };
        payload.FieldValues.Add(Varieties.Species, species);
        payload.FieldValues.Add(Varieties.IsDefault, variety.IsDefault);
        payload.FieldValues.Add(Varieties.CanChangeForm, variety.CanChangeForm);
        payload.FieldValues.Add(Varieties.GenderRatio, genderRatio);
        payload.FieldValues.Add(Varieties.Genus, variety.Genus);
        payload.FieldValues.Add(Varieties.Url, variety.Url);
        payload.FieldValues.Add(Varieties.Notes, variety.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The variety content 'Id={ContentId}' was created.", content.Id);
      }

      await _contentService.PublishAsync(content.Id, language: null, cancellationToken);
      await _contentService.PublishAsync(content.Id, task.Language, cancellationToken);
      _logger.LogInformation("The variety content 'Id={ContentId}' was published.", content.Id);
    }
  }
}
