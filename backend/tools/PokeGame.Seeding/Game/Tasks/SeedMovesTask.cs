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

      string type = SeedingSerializer.Serialize<string[]>([PokemonConverter.Instance.FromType(move.Type)]);
      string category = SeedingSerializer.Serialize<string[]>([PokemonConverter.Instance.FromMoveCategory(move.Category)]);
      string inflictedCondition = move.InflictedStatus is null
        ? string.Empty
        : SeedingSerializer.Serialize<string[]>([PokemonConverter.Instance.FromStatusCondition(move.InflictedStatus.Condition)]);
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
        invariant.FieldValues.Add(Moves.Type, type);
        invariant.FieldValues.Add(Moves.Category, category);
        invariant.FieldValues.Add(Moves.Accuracy, move.Accuracy);
        invariant.FieldValues.Add(Moves.Power, move.Power);
        invariant.FieldValues.Add(Moves.PowerPoints, move.PowerPoints);
        invariant.FieldValues.Add(Moves.InflictedCondition, inflictedCondition);
        invariant.FieldValues.Add(Moves.StatusChance, statusChance);
        invariant.FieldValues.Add(Moves.VolatileConditions, volatileConditions);
        AddStatisticChangeFieldValues(invariant.FieldValues, move.StatisticChanges);
        invariant.FieldValues.Add(Moves.CriticalChange, move.CriticalChange);
        _ = await _contentService.SaveLocaleAsync(move.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = move.UniqueName,
          DisplayName = move.DisplayName,
          Description = move.Description
        };
        locale.FieldValues.Add(Moves.Url, move.Url);
        locale.FieldValues.Add(Moves.Notes, move.Notes);
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
        payload.FieldValues.Add(Moves.Type, type);
        payload.FieldValues.Add(Moves.Category, category);
        payload.FieldValues.Add(Moves.Accuracy, move.Accuracy);
        payload.FieldValues.Add(Moves.Power, move.Power);
        payload.FieldValues.Add(Moves.PowerPoints, move.PowerPoints);
        payload.FieldValues.Add(Moves.InflictedCondition, inflictedCondition);
        payload.FieldValues.Add(Moves.StatusChance, statusChance);
        payload.FieldValues.Add(Moves.VolatileConditions, volatileConditions);
        AddStatisticChangeFieldValues(payload.FieldValues, move.StatisticChanges);
        payload.FieldValues.Add(Moves.Url, move.Url);
        payload.FieldValues.Add(Moves.Notes, move.Notes);
        payload.FieldValues.Add(Moves.CriticalChange, move.CriticalChange);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The move content 'Id={ContentId}' was created.", content.Id);
      }

      await _contentService.PublishAsync(content.Id, language: null, cancellationToken);
      await _contentService.PublishAsync(content.Id, task.Language, cancellationToken);
      _logger.LogInformation("The move content 'Id={ContentId}' was published.", content.Id);
    }
  }

  private static void AddStatisticChangeFieldValues(List<FieldValuePayload> fieldValues, StatisticChangesPayload changes)
  {
    fieldValues.Add(Moves.AttackChange, changes.Attack);
    fieldValues.Add(Moves.DefenseChange, changes.Defense);
    fieldValues.Add(Moves.SpecialAttackChange, changes.SpecialAttack);
    fieldValues.Add(Moves.SpecialDefenseChange, changes.SpecialDefense);
    fieldValues.Add(Moves.SpeedChange, changes.Speed);
    fieldValues.Add(Moves.AccuracyChange, changes.Accuracy);
    fieldValues.Add(Moves.EvasionChange, changes.Evasion);
  }

  private static string SerializeVolatileConditions(string? volatileConditions)
  {
    if (string.IsNullOrWhiteSpace(volatileConditions))
    {
      return string.Empty;
    }

    IEnumerable<VolatileCondition> values = volatileConditions.Split('|').Distinct().Select(Enum.Parse<VolatileCondition>);
    return SeedingSerializer.Serialize(values.Select(PokemonConverter.Instance.FromVolatileCondition));
  }
}
