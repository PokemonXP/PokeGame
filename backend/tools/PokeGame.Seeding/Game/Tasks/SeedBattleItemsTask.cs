using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using MediatR;
using PokeGame.Infrastructure.Data;
using PokeGame.Seeding.Game.Payloads;
using PokeGame.Seeding.Game.Validators;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedBattleItemsTask : SeedingTask
{
  public override string? Description => "Seeds Battle Item contents into Krakenar.";
  public string Language { get; }

  public SeedBattleItemsTask(string language)
  {
    Language = language;
  }
}

internal class SeedBattleItemsTaskHandler : INotificationHandler<SeedBattleItemsTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedBattleItemsTaskHandler> _logger;

  public SeedBattleItemsTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedBattleItemsTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedBattleItemsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<BattleItemPayload> battleItems = await CsvHelper.ExtractAsync<BattleItemPayload>("Game/data/items/battle.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = BattleItems.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    BattleItemValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (BattleItemPayload battleItem in battleItems)
    {
      ValidationResult result = validator.Validate(battleItem);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The battle item '{BattleItem}' was not seeded because there are validation errors.|Errors: {Errors}", battleItem, errors);
        continue;
      }

      Content content;
      if (existingIds.Contains(battleItem.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = battleItem.UniqueName,
          DisplayName = battleItem.DisplayName,
          Description = battleItem.Description
        };
        invariant.FieldValues.Add(BattleItems.Price, battleItem.Price);
        AddStatisticChangeFieldValues(invariant.FieldValues, battleItem.StatisticChanges);
        invariant.FieldValues.Add(BattleItems.CriticalChange, battleItem.CriticalChange);
        invariant.FieldValues.Add(BattleItems.GuardTurns, battleItem.GuardTurns);
        invariant.FieldValues.Add(BattleItems.Sprite, battleItem.Sprite);
        _ = await _contentService.SaveLocaleAsync(battleItem.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = battleItem.UniqueName,
          DisplayName = battleItem.DisplayName,
          Description = battleItem.Description
        };
        locale.FieldValues.Add(BattleItems.Url, battleItem.Url);
        locale.FieldValues.Add(BattleItems.Notes, battleItem.Notes);
        content = await _contentService.SaveLocaleAsync(battleItem.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The battle item content 'Id={battleItem.Id}' was not found.");
        _logger.LogInformation("The battle item content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = battleItem.Id,
          ContentType = BattleItems.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = battleItem.UniqueName,
          DisplayName = battleItem.DisplayName,
          Description = battleItem.Description
        };
        payload.FieldValues.Add(BattleItems.Price, battleItem.Price);
        AddStatisticChangeFieldValues(payload.FieldValues, battleItem.StatisticChanges);
        payload.FieldValues.Add(BattleItems.CriticalChange, battleItem.CriticalChange);
        payload.FieldValues.Add(BattleItems.GuardTurns, battleItem.GuardTurns);
        payload.FieldValues.Add(BattleItems.Sprite, battleItem.Sprite);
        payload.FieldValues.Add(BattleItems.Url, battleItem.Url);
        payload.FieldValues.Add(BattleItems.Notes, battleItem.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The battle item content 'Id={ContentId}' was created.", content.Id);
      }
    }
  }

  private static void AddStatisticChangeFieldValues(List<FieldValuePayload> fieldValues, StatisticChangesPayload changes)
  {
    fieldValues.Add(BattleItems.AttackChange, changes.Attack);
    fieldValues.Add(BattleItems.DefenseChange, changes.Defense);
    fieldValues.Add(BattleItems.SpecialAttackChange, changes.SpecialAttack);
    fieldValues.Add(BattleItems.SpecialDefenseChange, changes.SpecialDefense);
    fieldValues.Add(BattleItems.SpeedChange, changes.Speed);
    fieldValues.Add(BattleItems.AccuracyChange, changes.Accuracy);
    fieldValues.Add(BattleItems.EvasionChange, changes.Evasion);
  }
}
