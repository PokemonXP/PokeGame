using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using MediatR;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;

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
  private readonly IContentService _contentService;
  private readonly ILogger<SeedAbilitiesTaskHandler> _logger;

  public SeedAbilitiesTaskHandler(IContentService contentService, ILogger<SeedAbilitiesTaskHandler> logger)
  {
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedAbilitiesTask task, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Game/data/abilities.json", Encoding.UTF8, cancellationToken);
    IEnumerable<AbilityPayload>? abilities = SeedingSerializer.Deserialize<IEnumerable<AbilityPayload>>(json);
    if (abilities is not null)
    {
      SearchContentLocalesPayload search = new()
      {
        ContentTypeId = Abilities.ContentTypeId
      };
      SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
      HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

      foreach (AbilityPayload ability in abilities)
      {
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
          locale.FieldValues.Add(new FieldValuePayload(Abilities.Url.ToString(), ability.Url ?? string.Empty));
          locale.FieldValues.Add(new FieldValuePayload(Abilities.Notes.ToString(), ability.Notes ?? string.Empty));
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
          payload.FieldValues.Add(new FieldValuePayload(Abilities.Url.ToString(), ability.Url ?? string.Empty));
          payload.FieldValues.Add(new FieldValuePayload(Abilities.Notes.ToString(), ability.Notes ?? string.Empty));
          content = await _contentService.CreateAsync(payload, cancellationToken);
          _logger.LogInformation("The ability content 'Id={ContentId}' was created.", content.Id);
        }
      }
    }
  }
}
