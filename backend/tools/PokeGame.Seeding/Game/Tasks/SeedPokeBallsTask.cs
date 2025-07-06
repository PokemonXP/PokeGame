using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedPokeBallsTask : SeedingTask
{
  public override string? Description => "Seeds PokeBall contents into Krakenar.";
  public string Language { get; }

  public SeedPokeBallsTask(string language)
  {
    Language = language;
  }
}

internal class SeedPokeBallsTaskHandler : INotificationHandler<SeedPokeBallsTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedPokeBallsTaskHandler> _logger;

  public SeedPokeBallsTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedPokeBallsTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedPokeBallsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<PokeBallPayload> pokeBalls = await CsvHelper.ExtractAsync<PokeBallPayload>("Game/data/items/poke_balls.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = PokeBalls.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    PokeBallValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (PokeBallPayload pokeBall in pokeBalls)
    {
      ValidationResult result = validator.Validate(pokeBall);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The Poké Ball '{PokeBall}' was not seeded because there are validation errors.|Errors: {Errors}", pokeBall, errors);
        continue;
      }

      Content content;
      if (existingIds.Contains(pokeBall.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = pokeBall.UniqueName,
          DisplayName = pokeBall.DisplayName,
          Description = pokeBall.Description
        };
        invariant.FieldValues.Add(PokeBalls.Price, pokeBall.Price);
        invariant.FieldValues.Add(PokeBalls.CatchMultiplier, pokeBall.CatchMultiplier);
        invariant.FieldValues.Add(PokeBalls.Heal, pokeBall.Heal);
        invariant.FieldValues.Add(PokeBalls.BaseFriendship, pokeBall.BaseFriendship);
        invariant.FieldValues.Add(PokeBalls.FriendshipMultiplier, pokeBall.FriendshipMultiplier);
        invariant.FieldValues.Add(PokeBalls.Sprite, pokeBall.Sprite);
        _ = await _contentService.SaveLocaleAsync(pokeBall.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = pokeBall.UniqueName,
          DisplayName = pokeBall.DisplayName,
          Description = pokeBall.Description
        };
        locale.FieldValues.Add(PokeBalls.Url, pokeBall.Url);
        locale.FieldValues.Add(PokeBalls.Notes, pokeBall.Notes);
        content = await _contentService.SaveLocaleAsync(pokeBall.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The Poké Ball content 'Id={pokeBall.Id}' was not found.");
        _logger.LogInformation("The Poké Ball content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = pokeBall.Id,
          ContentType = PokeBalls.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = pokeBall.UniqueName,
          DisplayName = pokeBall.DisplayName,
          Description = pokeBall.Description
        };
        payload.FieldValues.Add(PokeBalls.Price, pokeBall.Price);
        payload.FieldValues.Add(PokeBalls.CatchMultiplier, pokeBall.CatchMultiplier);
        payload.FieldValues.Add(PokeBalls.Heal, pokeBall.Heal);
        payload.FieldValues.Add(PokeBalls.BaseFriendship, pokeBall.BaseFriendship);
        payload.FieldValues.Add(PokeBalls.FriendshipMultiplier, pokeBall.FriendshipMultiplier);
        payload.FieldValues.Add(PokeBalls.Sprite, pokeBall.Sprite);
        payload.FieldValues.Add(PokeBalls.Url, pokeBall.Url);
        payload.FieldValues.Add(PokeBalls.Notes, pokeBall.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The Poké Ball content 'Id={ContentId}' was created.", content.Id);
      }
    }
  }
}
