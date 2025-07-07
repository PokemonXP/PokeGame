using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedRegionsTask : SeedingTask
{
  public override string? Description => "Seeds Region contents into Krakenar.";
  public string Language { get; }

  public SeedRegionsTask(string language)
  {
    Language = language;
  }
}

internal class SeedRegionsTaskHandler : INotificationHandler<SeedRegionsTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedRegionsTaskHandler> _logger;

  public SeedRegionsTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedRegionsTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedRegionsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<RegionPayload> regions = await CsvHelper.ExtractAsync<RegionPayload>("Game/data/regions.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Regions.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    RegionValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (RegionPayload region in regions)
    {
      ValidationResult result = validator.Validate(region);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The region '{Region}' was not seeded because there are validation errors.|Errors: {Errors}", region, errors);
        continue;
      }

      Content content;
      if (existingIds.Contains(region.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = region.UniqueName,
          DisplayName = region.DisplayName,
          Description = region.Description
        };
        _ = await _contentService.SaveLocaleAsync(region.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = region.UniqueName,
          DisplayName = region.DisplayName,
          Description = region.Description
        };
        locale.FieldValues.Add(Regions.Url, region.Url);
        locale.FieldValues.Add(Regions.Notes, region.Notes);
        content = await _contentService.SaveLocaleAsync(region.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The region content 'Id={region.Id}' was not found.");
        _logger.LogInformation("The region content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = region.Id,
          ContentType = Regions.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = region.UniqueName,
          DisplayName = region.DisplayName,
          Description = region.Description
        };
        payload.FieldValues.Add(Regions.Url, region.Url);
        payload.FieldValues.Add(Regions.Notes, region.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The region content 'Id={ContentId}' was created.", content.Id);
      }

      await _contentService.PublishAsync(content.Id, language: null, cancellationToken);
      await _contentService.PublishAsync(content.Id, task.Language, cancellationToken);
      _logger.LogInformation("The region content 'Id={ContentId}' was published.", content.Id);
    }
  }
}
