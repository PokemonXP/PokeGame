using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedAbilitiesTask : SeedingTask
{
  public override string? Description => "Seeds Ability contents into Krakenar.";
  public string Language { get; }

  public SeedAbilitiesTask(string language)
  {
    Language = language;
  }
}

internal class SeedAbilitiesTaskHandler : INotificationHandler<SeedAbilitiesTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedAbilitiesTaskHandler> _logger;

  public SeedAbilitiesTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedAbilitiesTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedAbilitiesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<AbilityPayload> abilities = await CsvHelper.ExtractAsync<AbilityPayload>("Game/data/abilities.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Abilities.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    AbilityValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (AbilityPayload ability in abilities)
    {
      ValidationResult result = validator.Validate(ability);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The ability '{Ability}' was not seeded because there are validation errors.|Errors: {Errors}", ability, errors);
        continue;
      }

      Content content;
      if (existingIds.Contains(ability.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = ability.UniqueName,
          DisplayName = ability.DisplayName,
          Description = ability.Description
        };
        _ = await _contentService.SaveLocaleAsync(ability.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = ability.UniqueName,
          DisplayName = ability.DisplayName,
          Description = ability.Description
        };
        locale.FieldValues.Add(Abilities.Url, ability.Url);
        locale.FieldValues.Add(Abilities.Notes, ability.Notes);
        content = await _contentService.SaveLocaleAsync(ability.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The ability content 'Id={ability.Id}' was not found.");
        _logger.LogInformation("The ability content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = ability.Id,
          ContentType = Abilities.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = ability.UniqueName,
          DisplayName = ability.DisplayName,
          Description = ability.Description
        };
        payload.FieldValues.Add(Abilities.Url, ability.Url);
        payload.FieldValues.Add(Abilities.Notes, ability.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The ability content 'Id={ContentId}' was created.", content.Id);
      }
    }
  }
}
