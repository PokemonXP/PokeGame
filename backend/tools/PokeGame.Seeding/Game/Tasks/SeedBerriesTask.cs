using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Core;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedBerriesTask : SeedingTask
{
  public override string? Description => "Seeds Berry contents into Krakenar.";
  public string Language { get; }

  public SeedBerriesTask(string language)
  {
    Language = language;
  }
}

internal class SeedBerriesTaskHandler : INotificationHandler<SeedBerriesTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedBerriesTaskHandler> _logger;

  public SeedBerriesTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedBerriesTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedBerriesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<BerryPayload> berries = await CsvHelper.ExtractAsync<BerryPayload>("Game/data/items/berries.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Berries.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    BerryValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (BerryPayload berry in berries)
    {
      ValidationResult result = validator.Validate(berry);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The berry '{Berry}' was not seeded because there are validation errors.|Errors: {Errors}", berry, errors);
        continue;
      }

      string statusCondition = berry.StatusCondition.HasValue
        ? SeedingSerializer.Serialize<StatusCondition[]>([berry.StatusCondition.Value]).ToLowerInvariant()
        : string.Empty;
      string lowerEffortValues = FormatPokemonStatistic(berry.LowerEffortValues);

      Content content;
      if (existingIds.Contains(berry.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = berry.UniqueName,
          DisplayName = berry.DisplayName,
          Description = berry.Description
        };
        invariant.FieldValues.Add(Berries.Price, berry.Price);
        invariant.FieldValues.Add(Berries.Healing, berry.Healing);
        invariant.FieldValues.Add(Berries.IsHealingPercentage, berry.IsHealingPercentage);
        invariant.FieldValues.Add(Berries.StatusCondition, statusCondition);
        invariant.FieldValues.Add(Berries.CureConfusion, berry.CureConfusion);
        invariant.FieldValues.Add(Berries.CureNonVolatileConditions, berry.CureNonVolatileConditions);
        invariant.FieldValues.Add(Berries.PowerPoints, berry.PowerPoints);
        AddStatisticChangeFieldValues(invariant.FieldValues, berry.StatisticChanges);
        invariant.FieldValues.Add(Berries.CriticalChange, berry.CriticalChange);
        invariant.FieldValues.Add(Berries.LowerEffortValues, lowerEffortValues);
        invariant.FieldValues.Add(Berries.RaiseFriendship, berry.RaiseFriendship);
        invariant.FieldValues.Add(Berries.Sprite, berry.Sprite);
        _ = await _contentService.SaveLocaleAsync(berry.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = berry.UniqueName,
          DisplayName = berry.DisplayName,
          Description = berry.Description
        };
        locale.FieldValues.Add(Berries.Url, berry.Url);
        locale.FieldValues.Add(Berries.Notes, berry.Notes);
        content = await _contentService.SaveLocaleAsync(berry.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The berry content 'Id={berry.Id}' was not found.");
        _logger.LogInformation("The berry content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = berry.Id,
          ContentType = Berries.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = berry.UniqueName,
          DisplayName = berry.DisplayName,
          Description = berry.Description
        };
        payload.FieldValues.Add(Berries.Price, berry.Price);
        payload.FieldValues.Add(Berries.Price, berry.Price);
        payload.FieldValues.Add(Berries.Healing, berry.Healing);
        payload.FieldValues.Add(Berries.IsHealingPercentage, berry.IsHealingPercentage);
        payload.FieldValues.Add(Berries.StatusCondition, statusCondition);
        payload.FieldValues.Add(Berries.CureConfusion, berry.CureConfusion);
        payload.FieldValues.Add(Berries.CureNonVolatileConditions, berry.CureNonVolatileConditions);
        payload.FieldValues.Add(Berries.PowerPoints, berry.PowerPoints);
        AddStatisticChangeFieldValues(payload.FieldValues, berry.StatisticChanges);
        payload.FieldValues.Add(Berries.CriticalChange, berry.CriticalChange);
        payload.FieldValues.Add(Berries.LowerEffortValues, lowerEffortValues);
        payload.FieldValues.Add(Berries.RaiseFriendship, berry.RaiseFriendship);
        payload.FieldValues.Add(Berries.Sprite, berry.Sprite);
        payload.FieldValues.Add(Berries.Sprite, berry.Sprite);
        payload.FieldValues.Add(Berries.Url, berry.Url);
        payload.FieldValues.Add(Berries.Notes, berry.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The berry content 'Id={ContentId}' was created.", content.Id);
      }
    }
  }

  private static void AddStatisticChangeFieldValues(List<FieldValuePayload> fieldValues, StatisticChangesPayload changes)
  {
    fieldValues.Add(Berries.AttackChange, changes.Attack);
    fieldValues.Add(Berries.DefenseChange, changes.Defense);
    fieldValues.Add(Berries.SpecialAttackChange, changes.SpecialAttack);
    fieldValues.Add(Berries.SpecialDefenseChange, changes.SpecialDefense);
    fieldValues.Add(Berries.SpeedChange, changes.Speed);
    fieldValues.Add(Berries.AccuracyChange, changes.Accuracy);
    fieldValues.Add(Berries.EvasionChange, changes.Evasion);
  }

  private static string FormatPokemonStatistic(PokemonStatistic? statistic)
  {
    if (!statistic.HasValue)
    {
      return string.Empty;
    }

    List<string> values = new(capacity: 1);
    switch (statistic.Value)
    {
      case PokemonStatistic.Attack:
        values.Add("attack");
        break;
      case PokemonStatistic.Defense:
        values.Add("defense");
        break;
      case PokemonStatistic.HP:
        values.Add("hp");
        break;
      case PokemonStatistic.SpecialAttack:
        values.Add("special-attack");
        break;
      case PokemonStatistic.SpecialDefense:
        values.Add("special-defense");
        break;
      case PokemonStatistic.Speed:
        values.Add("speed");
        break;
      default:
        throw new NotSupportedException($"The Pokémon statistic '{statistic}' is not supported.");
    }
    return SeedingSerializer.Serialize(values);
  }
}
