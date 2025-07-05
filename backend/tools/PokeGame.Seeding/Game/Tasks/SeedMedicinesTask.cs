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

internal class SeedMedicinesTask : SeedingTask
{
  public override string? Description => "Seeds Medicine contents into Krakenar.";
  public string Language { get; }

  public SeedMedicinesTask(string language)
  {
    Language = language;
  }
}

internal class SeedMedicinesTaskHandler : INotificationHandler<SeedMedicinesTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedMedicinesTaskHandler> _logger;

  public SeedMedicinesTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedMedicinesTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedMedicinesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<MedicinePayload> medicines = await CsvHelper.ExtractAsync<MedicinePayload>("Game/data/items/medicines.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Medicines.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    MedicineValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (MedicinePayload medicine in medicines)
    {
      ValidationResult result = validator.Validate(medicine);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The medicine '{Medicine}' was not seeded because there are validation errors.|Errors: {Errors}", medicine, errors);
        continue;
      }

      Content content;
      if (existingIds.Contains(medicine.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = medicine.UniqueName,
          DisplayName = medicine.DisplayName,
          Description = medicine.Description
        };
        invariant.FieldValues.Add(Medicines.Price, medicine.Price);
        invariant.FieldValues.Add(Medicines.IsHerbal, medicine.IsHerbal);
        AddHealingFieldValues(invariant.FieldValues, medicine.Healing);
        AddStatusFieldValues(invariant.FieldValues, medicine.Status);
        AddPowerPointsFieldValues(invariant.FieldValues, medicine.PowerPoints);
        invariant.FieldValues.Add(Medicines.Sprite, medicine.Sprite);
        _ = await _contentService.SaveLocaleAsync(medicine.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = medicine.UniqueName,
          DisplayName = medicine.DisplayName,
          Description = medicine.Description
        };
        locale.FieldValues.Add(Medicines.Url, medicine.Url);
        locale.FieldValues.Add(Medicines.Notes, medicine.Notes);
        content = await _contentService.SaveLocaleAsync(medicine.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The medicine content 'Id={medicine.Id}' was not found.");
        _logger.LogInformation("The medicine content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = medicine.Id,
          ContentType = Medicines.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = medicine.UniqueName,
          DisplayName = medicine.DisplayName,
          Description = medicine.Description
        };
        payload.FieldValues.Add(Medicines.Price, medicine.Price);
        payload.FieldValues.Add(Medicines.IsHerbal, medicine.IsHerbal);
        AddHealingFieldValues(payload.FieldValues, medicine.Healing);
        AddStatusFieldValues(payload.FieldValues, medicine.Status);
        AddPowerPointsFieldValues(payload.FieldValues, medicine.PowerPoints);
        payload.FieldValues.Add(Medicines.Sprite, medicine.Sprite);
        payload.FieldValues.Add(Medicines.Url, medicine.Url);
        payload.FieldValues.Add(Medicines.Notes, medicine.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The medicine content 'Id={ContentId}' was created.", content.Id);
      }
    }
  }

  private static void AddHealingFieldValues(List<FieldValuePayload> fieldValues, HealingPayload? healing)
  {
    if (healing is null)
    {
      return;
    }

    fieldValues.Add(Medicines.Healing, healing.Value);
    fieldValues.Add(Medicines.IsHealingPercentage, healing.IsPercentage);
    fieldValues.Add(Medicines.Revive, healing.Revive);
  }

  private static void AddPowerPointsFieldValues(List<FieldValuePayload> fieldValues, PowerPointRestorePayload? powerPoints)
  {
    if (powerPoints is null)
    {
      return;
    }

    fieldValues.Add(Medicines.PowerPoints, powerPoints.Value);
    fieldValues.Add(Medicines.IsPowerPointPercentage, powerPoints.IsPercentage);
    fieldValues.Add(Medicines.AllMoves, powerPoints.AllMoves);
  }

  private static void AddStatusFieldValues(List<FieldValuePayload> fieldValues, StatusHealingPayload? status)
  {
    if (status is null)
    {
      return;
    }

    if (status.Condition.HasValue)
    {
      string statusCondition = SeedingSerializer.Serialize<StatusCondition[]>([status.Condition.Value]).ToLowerInvariant();
      fieldValues.Add(Medicines.StatusCondition, statusCondition);
    }
    fieldValues.Add(Medicines.AllConditions, status.All);
  }
}
