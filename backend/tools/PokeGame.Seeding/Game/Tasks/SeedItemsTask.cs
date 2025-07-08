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

internal class SeedItemsTask : SeedingTask
{
  public override string? Description => "Seeds Item contents into Krakenar.";
  public string Language { get; }

  public SeedItemsTask(string language)
  {
    Language = language;
  }
}

internal class SeedItemsTaskHandler : INotificationHandler<SeedItemsTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedItemsTaskHandler> _logger;

  public SeedItemsTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedItemsTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedItemsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<ItemPayload> items = await CsvHelper.ExtractAsync<ItemPayload>("Game/data/items/other.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Items.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    ItemValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (ItemPayload item in items)
    {
      ValidationResult result = validator.Validate(item);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The item '{Item}' was not seeded because there are validation errors.|Errors: {Errors}", item, errors);
        continue;
      }

      string category = item.Category.HasValue
        ? SeedingSerializer.Serialize<string[]>([PokemonConverter.Instance.FromItemCategory(item.Category.Value)])
        : string.Empty;

      Content content;
      if (existingIds.Contains(item.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = item.UniqueName,
          DisplayName = item.DisplayName,
          Description = item.Description
        };
        invariant.FieldValues.Add(Items.Price, item.Price);
        invariant.FieldValues.Add(Items.Category, category);
        invariant.FieldValues.Add(Items.Sprite, item.Sprite);
        _ = await _contentService.SaveLocaleAsync(item.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = item.UniqueName,
          DisplayName = item.DisplayName,
          Description = item.Description
        };
        locale.FieldValues.Add(Items.Url, item.Url);
        locale.FieldValues.Add(Items.Notes, item.Notes);
        content = await _contentService.SaveLocaleAsync(item.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The item content 'Id={item.Id}' was not found.");
        _logger.LogInformation("The item content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = item.Id,
          ContentType = Items.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = item.UniqueName,
          DisplayName = item.DisplayName,
          Description = item.Description
        };
        payload.FieldValues.Add(Items.Price, item.Price);
        payload.FieldValues.Add(Items.Category, category);
        payload.FieldValues.Add(Items.Sprite, item.Sprite);
        payload.FieldValues.Add(Items.Url, item.Url);
        payload.FieldValues.Add(Items.Notes, item.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The item content 'Id={ContentId}' was created.", content.Id);
      }

      await _contentService.PublishAsync(content.Id, language: null, cancellationToken);
      await _contentService.PublishAsync(content.Id, task.Language, cancellationToken);
      _logger.LogInformation("The item content 'Id={ContentId}' was published.", content.Id);
    }
  }
}
