using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Core.Species;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedSpeciesTask : SeedingTask
{
  public override string? Description => "Seeds Species contents into Krakenar.";
  public string Language { get; }

  public SeedSpeciesTask(string language)
  {
    Language = language;
  }
}

internal class SeedSpeciesTaskHandler : INotificationHandler<SeedSpeciesTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedSpeciesTaskHandler> _logger;

  public SeedSpeciesTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedSpeciesTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedSpeciesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SpeciesPayload> speciesList = await CsvHelper.ExtractAsync<SpeciesPayload>("Game/data/species.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Species.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    SpeciesValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (SpeciesPayload species in speciesList)
    {
      ValidationResult result = validator.Validate(species);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The species '{Species}' was not seeded because there are validation errors.|Errors: {Errors}", species, errors);
        continue;
      }

      string category = SeedingSerializer.Serialize<PokemonCategory[]>([species.Category]).ToLowerInvariant();
      string growthRate = FormatGrowthRate(species.GrowthRate);
      string regionalNumbers = FormatRegionalNumbers(species.RegionalNumbers);

      Content content;
      if (existingIds.Contains(species.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = species.UniqueName,
          DisplayName = species.DisplayName
        };
        invariant.FieldValues.Add(Species.Number, species.Number);
        invariant.FieldValues.Add(Species.Category, category);
        invariant.FieldValues.Add(Species.BaseFriendship, species.BaseFriendship);
        invariant.FieldValues.Add(Species.CatchRate, species.CatchRate);
        invariant.FieldValues.Add(Species.GrowthRate, growthRate);
        invariant.FieldValues.Add(Species.RegionalNumbers, regionalNumbers);
        _ = await _contentService.SaveLocaleAsync(species.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = species.UniqueName,
          DisplayName = species.DisplayName
        };
        locale.FieldValues.Add(Species.Url, species.Url);
        locale.FieldValues.Add(Species.Notes, species.Notes);
        content = await _contentService.SaveLocaleAsync(species.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The species content 'Id={species.Id}' was not found.");
        _logger.LogInformation("The species content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = species.Id,
          ContentType = Species.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = species.UniqueName,
          DisplayName = species.DisplayName
        };
        payload.FieldValues.Add(Species.Number, species.Number);
        payload.FieldValues.Add(Species.Category, category);
        payload.FieldValues.Add(Species.BaseFriendship, species.BaseFriendship);
        payload.FieldValues.Add(Species.CatchRate, species.CatchRate);
        payload.FieldValues.Add(Species.GrowthRate, growthRate);
        payload.FieldValues.Add(Species.RegionalNumbers, regionalNumbers);
        payload.FieldValues.Add(Species.Url, species.Url);
        payload.FieldValues.Add(Species.Notes, species.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The species content 'Id={ContentId}' was created.", content.Id);
      }

      await _contentService.PublishAsync(content.Id, language: null, cancellationToken);
      await _contentService.PublishAsync(content.Id, task.Language, cancellationToken);
      _logger.LogInformation("The species content 'Id={ContentId}' was published.", content.Id);
    }
  }

  private static string FormatGrowthRate(GrowthRate growthRate)
  {
    string value = growthRate switch
    {
      GrowthRate.Erratic => "slow-then-very-fast",
      GrowthRate.Fast => "fast",
      GrowthRate.Fluctuating => "fast-then-very-slow",
      GrowthRate.MediumFast => "medium",
      GrowthRate.MediumSlow => "medium-slow",
      GrowthRate.Slow => "slow",
      _ => throw new ArgumentException($"The growth rate '{growthRate}' is not valid.", nameof(growthRate)),
    };
    return SeedingSerializer.Serialize<string[]>([value]);
  }

  private static string FormatRegionalNumbers(IEnumerable<RegionalNumberPayload> payloads)
  {
    Dictionary<string, int> regionalNumbers = new(capacity: payloads.Count());
    foreach (RegionalNumberPayload payload in payloads)
    {
      regionalNumbers[payload.Region.Trim().ToLowerInvariant()] = payload.Number;
    }
    return string.Join('|', regionalNumbers.Select(x => string.Join(':', x.Key, x.Value)));
  }
}
