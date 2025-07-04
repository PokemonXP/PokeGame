using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedMovesTask : SeedingTask
{
  public override string? Description => "Seeds Move contents into Krakenar.";
  public string Language { get; }

  public SeedMovesTask(string language)
  {
    Language = language;
  }
}

internal class SeedMovesTaskHandler : INotificationHandler<SeedMovesTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedMovesTaskHandler> _logger;

  public SeedMovesTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedMovesTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedMovesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<MovePayload> moves = await CsvHelper.ExtractAsync<MovePayload>("Game/data/moves.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Moves.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    MoveValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (MovePayload move in moves)
    {
      ValidationResult result = validator.Validate(move);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The move '{Move}' was not seeded because there are validation errors.|Errors: {Errors}", move, errors);
        continue;
      }

      string type = SeedingSerializer.Serialize<PokemonType[]>([move.Type]).ToLowerInvariant();
      string category = SeedingSerializer.Serialize<MoveCategory[]>([move.Category]).ToLowerInvariant();
      string inflictedCondition = move.InflictedStatus is null ? string.Empty : SeedingSerializer.Serialize<StatusCondition[]>([move.InflictedStatus.Condition]).ToLowerInvariant();
      string statusChance = move.InflictedStatus is null ? string.Empty : move.InflictedStatus.Chance.ToString();
      string volatileConditions = SerializeVolatileConditions(move.VolatileConditions);

      Content content;
      if (existingIds.Contains(move.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = move.UniqueName,
          DisplayName = move.DisplayName,
          Description = move.Description
        };
        invariant.FieldValues.Add(new FieldValuePayload(Moves.Type.ToString(), type));
        invariant.FieldValues.Add(new FieldValuePayload(Moves.Category.ToString(), category));
        invariant.FieldValues.Add(new FieldValuePayload(Moves.Accuracy.ToString(), move.Accuracy?.ToString() ?? string.Empty));
        invariant.FieldValues.Add(new FieldValuePayload(Moves.Power.ToString(), move.Power?.ToString() ?? string.Empty));
        invariant.FieldValues.Add(new FieldValuePayload(Moves.PowerPoints.ToString(), move.PowerPoints.ToString()));
        invariant.FieldValues.Add(new FieldValuePayload(Moves.InflictedCondition.ToString(), inflictedCondition));
        invariant.FieldValues.Add(new FieldValuePayload(Moves.StatusChance.ToString(), statusChance));
        invariant.FieldValues.Add(new FieldValuePayload(Moves.VolatileConditions.ToString(), volatileConditions));
        invariant.FieldValues.AddRange(GetStatisticChanges(move.StatisticChanges));
        _ = await _contentService.SaveLocaleAsync(move.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = move.UniqueName,
          DisplayName = move.DisplayName,
          Description = move.Description
        };
        locale.FieldValues.Add(new FieldValuePayload(Moves.Url.ToString(), move.Url ?? string.Empty));
        locale.FieldValues.Add(new FieldValuePayload(Moves.Notes.ToString(), move.Notes ?? string.Empty));
        content = await _contentService.SaveLocaleAsync(move.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The move content 'Id={move.Id}' was not found.");
        _logger.LogInformation("The move content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = move.Id,
          ContentType = Moves.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = move.UniqueName,
          DisplayName = move.DisplayName,
          Description = move.Description
        };
        payload.FieldValues.Add(new FieldValuePayload(Moves.Type.ToString(), type));
        payload.FieldValues.Add(new FieldValuePayload(Moves.Category.ToString(), category));
        payload.FieldValues.Add(new FieldValuePayload(Moves.Accuracy.ToString(), move.Accuracy?.ToString() ?? string.Empty));
        payload.FieldValues.Add(new FieldValuePayload(Moves.Power.ToString(), move.Power?.ToString() ?? string.Empty));
        payload.FieldValues.Add(new FieldValuePayload(Moves.PowerPoints.ToString(), move.PowerPoints.ToString()));
        payload.FieldValues.Add(new FieldValuePayload(Moves.InflictedCondition.ToString(), inflictedCondition));
        payload.FieldValues.Add(new FieldValuePayload(Moves.StatusChance.ToString(), statusChance));
        payload.FieldValues.Add(new FieldValuePayload(Moves.VolatileConditions.ToString(), volatileConditions));
        payload.FieldValues.AddRange(GetStatisticChanges(move.StatisticChanges));
        payload.FieldValues.Add(new FieldValuePayload(Moves.Url.ToString(), move.Url ?? string.Empty));
        payload.FieldValues.Add(new FieldValuePayload(Moves.Notes.ToString(), move.Notes ?? string.Empty));
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The move content 'Id={ContentId}' was created.", content.Id);
      }
    }
  }

  private static IReadOnlyCollection<FieldValuePayload> GetStatisticChanges(StatisticChangesPayload changes) => new FieldValuePayload[]
  {
    new(Moves.AttackChange.ToString(), changes.Attack.ToString()),
    new(Moves.DefenseChange.ToString(), changes.Defense.ToString()),
    new(Moves.SpecialAttackChange.ToString(), changes.SpecialAttack.ToString()),
    new(Moves.SpecialDefenseChange.ToString(), changes.SpecialDefense.ToString()),
    new(Moves.SpeedChange.ToString(), changes.Speed.ToString()),
    new(Moves.AccuracyChange.ToString(), changes.Accuracy.ToString()),
    new(Moves.EvasionChange.ToString(), changes.Evasion.ToString())
  };

  private static string SerializeVolatileConditions(string? volatileConditions)
  {
    if (string.IsNullOrWhiteSpace(volatileConditions))
    {
      return string.Empty;
    }

    return SeedingSerializer.Serialize(volatileConditions.Split('|').Distinct().Select(Enum.Parse<VolatileCondition>)).ToLowerInvariant();
  }
}
