using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
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
    string json = await File.ReadAllTextAsync("Game/data/species.json", Encoding.UTF8, cancellationToken);
    IEnumerable<SpeciesPayload>? speciesList = SeedingSerializer.Deserialize<IEnumerable<SpeciesPayload>>(json);
    if (speciesList is not null)
    {
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
          invariant.FieldValues.Add(new FieldValuePayload(Species.Number.ToString(), species.Number.ToString()));
          invariant.FieldValues.Add(new FieldValuePayload(Species.Category.ToString(), category));
          invariant.FieldValues.Add(new FieldValuePayload(Species.BaseFriendship.ToString(), species.BaseFriendship.ToString()));
          invariant.FieldValues.Add(new FieldValuePayload(Species.CatchRate.ToString(), species.CatchRate.ToString()));
          invariant.FieldValues.Add(new FieldValuePayload(Species.GrowthRate.ToString(), growthRate));
          invariant.FieldValues.Add(new FieldValuePayload(Species.RegionalNumbers.ToString(), regionalNumbers));
          _ = await _contentService.SaveLocaleAsync(species.Id, invariant, language: null, cancellationToken);

          SaveContentLocalePayload locale = new()
          {
            UniqueName = species.UniqueName,
            DisplayName = species.DisplayName
          };
          locale.FieldValues.Add(new FieldValuePayload(Species.Url.ToString(), species.Url ?? string.Empty));
          locale.FieldValues.Add(new FieldValuePayload(Species.Notes.ToString(), species.Notes ?? string.Empty));
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
          payload.FieldValues.Add(new FieldValuePayload(Species.Number.ToString(), species.Number.ToString()));
          payload.FieldValues.Add(new FieldValuePayload(Species.Category.ToString(), category));
          payload.FieldValues.Add(new FieldValuePayload(Species.BaseFriendship.ToString(), species.BaseFriendship.ToString()));
          payload.FieldValues.Add(new FieldValuePayload(Species.CatchRate.ToString(), species.CatchRate.ToString()));
          payload.FieldValues.Add(new FieldValuePayload(Species.GrowthRate.ToString(), growthRate));
          payload.FieldValues.Add(new FieldValuePayload(Species.RegionalNumbers.ToString(), regionalNumbers));
          payload.FieldValues.Add(new FieldValuePayload(Species.Url.ToString(), species.Url ?? string.Empty));
          payload.FieldValues.Add(new FieldValuePayload(Species.Notes.ToString(), species.Notes ?? string.Empty));
          content = await _contentService.CreateAsync(payload, cancellationToken);
          _logger.LogInformation("The species content 'Id={ContentId}' was created.", content.Id);
        }
      }
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
      _ => throw new NotSupportedException($"The growth rate '{growthRate}' is not supported."),
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
