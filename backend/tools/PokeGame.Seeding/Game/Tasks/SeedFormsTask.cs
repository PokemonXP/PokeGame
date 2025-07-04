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

internal class SeedFormsTask : SeedingTask
{
  public override string? Description => "Seeds Form contents into Krakenar.";
  public string Language { get; }

  public SeedFormsTask(string language)
  {
    Language = language;
  }
}

internal class SeedFormsTaskHandler : INotificationHandler<SeedFormsTask>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentService _contentService;
  private readonly ILogger<SeedFormsTaskHandler> _logger;

  public SeedFormsTaskHandler(IApplicationContext applicationContext, IContentService contentService, ILogger<SeedFormsTaskHandler> logger)
  {
    _applicationContext = applicationContext;
    _contentService = contentService;
    _logger = logger;
  }

  public async Task Handle(SeedFormsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<FormPayload> forms = await CsvHelper.ExtractAsync<FormPayload>("Game/data/forms.csv", cancellationToken);

    SearchContentLocalesPayload search = new()
    {
      ContentTypeId = Varieties.ContentTypeId
    };
    SearchResults<ContentLocale> results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    ContentIndex varietiesIndex = new(results); // TODO(fpion): should only load published?

    search.ContentTypeId = Abilities.ContentTypeId;
    results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    ContentIndex abilitiesIndex = new(results); // TODO(fpion): should only load published?

    search.ContentTypeId = Forms.ContentTypeId;
    results = await _contentService.SearchLocalesAsync(search, cancellationToken);
    HashSet<Guid> existingIds = results.Items.Select(locale => locale.Content.Id).ToHashSet();

    FormValidator validator = new(_applicationContext.UniqueNameSettings);
    foreach (FormPayload form in forms)
    {
      ValidationResult result = validator.Validate(form);
      if (!result.IsValid)
      {
        string errors = SeedingSerializer.Serialize(result.Errors);
        _logger.LogError("The form '{Form}' was not seeded because there are validation errors.|Errors: {Errors}", form, errors);
        continue;
      }

      Guid? varietyId = varietiesIndex.Get(form.Variety)?.Content.Id;
      if (!varietyId.HasValue)
      {
        _logger.LogError("The form '{Form}' was not seeded because the variety '{Variety}' was not found.", form, form.Variety);
        continue;
      }

      string variety = SeedingSerializer.Serialize<Guid[]>([varietyId.Value]);

      Content content;
      if (existingIds.Contains(form.Id))
      {
        SaveContentLocalePayload invariant = new()
        {
          UniqueName = form.UniqueName,
          DisplayName = form.DisplayName,
          Description = form.Description
        };
        invariant.FieldValues.Add(Forms.Variety, variety);
        invariant.FieldValues.Add(Forms.IsDefault, form.IsDefault);
        invariant.FieldValues.Add(Forms.IsBattleOnly, form.IsBattleOnly);
        invariant.FieldValues.Add(Forms.IsMega, form.IsMega);
        invariant.FieldValues.Add(Forms.Height, form.Height);
        invariant.FieldValues.Add(Forms.Weight, form.Weight);
        AddTypeFieldValues(invariant.FieldValues, form.Types);
        if (!AddAbilityFieldValues(invariant.FieldValues, form, abilitiesIndex))
        {
          continue;
        }
        AddBaseStatisticsFieldValues(invariant.FieldValues, form.BaseStatistics);
        AddYieldFieldValues(invariant.FieldValues, form.Yield);
        AddSpritesFieldValues(invariant.FieldValues, form.Sprites);
        _ = await _contentService.SaveLocaleAsync(form.Id, invariant, language: null, cancellationToken);

        SaveContentLocalePayload locale = new()
        {
          UniqueName = form.UniqueName,
          DisplayName = form.DisplayName,
          Description = form.Description
        };
        locale.FieldValues.Add(Forms.Url, form.Url);
        locale.FieldValues.Add(Forms.Notes, form.Notes);
        content = await _contentService.SaveLocaleAsync(form.Id, locale, task.Language, cancellationToken)
          ?? throw new InvalidOperationException($"The form content 'Id={form.Id}' was not found.");
        _logger.LogInformation("The form content 'Id={ContentId}' was updated.", content.Id);
      }
      else
      {
        CreateContentPayload payload = new()
        {
          Id = form.Id,
          ContentType = Forms.ContentTypeId.ToString(),
          Language = task.Language,
          UniqueName = form.UniqueName,
          DisplayName = form.DisplayName,
          Description = form.Description
        };
        payload.FieldValues.Add(Forms.Variety, variety);
        payload.FieldValues.Add(Forms.IsDefault, form.IsDefault);
        payload.FieldValues.Add(Forms.IsBattleOnly, form.IsBattleOnly);
        payload.FieldValues.Add(Forms.IsMega, form.IsMega);
        payload.FieldValues.Add(Forms.Height, form.Height);
        payload.FieldValues.Add(Forms.Weight, form.Weight);
        AddTypeFieldValues(payload.FieldValues, form.Types);
        if (!AddAbilityFieldValues(payload.FieldValues, form, abilitiesIndex))
        {
          continue;
        }
        AddBaseStatisticsFieldValues(payload.FieldValues, form.BaseStatistics);
        AddYieldFieldValues(payload.FieldValues, form.Yield);
        AddSpritesFieldValues(payload.FieldValues, form.Sprites);
        payload.FieldValues.Add(Forms.Url, form.Url);
        payload.FieldValues.Add(Forms.Notes, form.Notes);
        content = await _contentService.CreateAsync(payload, cancellationToken);
        _logger.LogInformation("The form content 'Id={ContentId}' was created.", content.Id);
      }
    }
  }

  private bool AddAbilityFieldValues(List<FieldValuePayload> fieldValues, FormPayload form, ContentIndex index)
  {
    AbilitiesPayload abilities = form.Abilities;
    Dictionary<Guid, int> counts = new(capacity: 3);

    Guid? primaryAbilityId = index.Get(abilities.Primary)?.Content.Id;
    if (!primaryAbilityId.HasValue)
    {
      _logger.LogError("The form '{Form}' was not seeded because the primary ability '{Ability}' was not found.", form, abilities.Primary);
      return false;
    }
    fieldValues.Add(Forms.PrimaryAbility, SeedingSerializer.Serialize<Guid[]>([primaryAbilityId.Value]));
    counts[primaryAbilityId.Value] = 1;

    Guid? secondaryAbilityId;
    if (!string.IsNullOrWhiteSpace(abilities.Secondary))
    {
      secondaryAbilityId = index.Get(abilities.Secondary)?.Content.Id;
      if (!secondaryAbilityId.HasValue)
      {
        _logger.LogError("The form '{Form}' was not seeded because the secondary ability '{Ability}' was not found.", form, abilities.Secondary);
        return false;
      }
      fieldValues.Add(Forms.SecondaryAbility, SeedingSerializer.Serialize<Guid[]>([secondaryAbilityId.Value]));

      _ = counts.TryGetValue(secondaryAbilityId.Value, out int count);
      counts[secondaryAbilityId.Value] = count + 1;
    }

    Guid? hiddenAbilityId;
    if (!string.IsNullOrWhiteSpace(abilities.Hidden))
    {
      hiddenAbilityId = index.Get(abilities.Hidden)?.Content.Id;
      if (!hiddenAbilityId.HasValue)
      {
        _logger.LogError("The form '{Form}' was not seeded because the hidden ability '{Ability}' was not found.", form, abilities.Hidden);
        return false;
      }
      fieldValues.Add(Forms.HiddenAbility, SeedingSerializer.Serialize<Guid[]>([hiddenAbilityId.Value]));

      _ = counts.TryGetValue(hiddenAbilityId.Value, out int count);
      counts[hiddenAbilityId.Value] = count + 1;
    }

    IEnumerable<Guid> duplicateIds = counts.Where(x => x.Value > 1).Select(x => x.Key).Distinct();
    if (duplicateIds.Any())
    {
      string duplicates = string.Join(", ", duplicateIds);
      _logger.LogError("The form '{Form}' was not seeded because the following abilities are duplicated: {Duplicates}.", form, duplicates);
      return false;
    }

    return true;
  }

  private static void AddBaseStatisticsFieldValues(List<FieldValuePayload> fieldValues, BaseStatisticsPayload @base)
  {
    fieldValues.Add(Forms.HPBase, @base.HP);
    fieldValues.Add(Forms.AttackBase, @base.Attack);
    fieldValues.Add(Forms.DefenseBase, @base.Defense);
    fieldValues.Add(Forms.SpecialAttackBase, @base.SpecialAttack);
    fieldValues.Add(Forms.SpecialDefenseBase, @base.SpecialDefense);
    fieldValues.Add(Forms.SpeedBase, @base.Speed);
  }

  private static void AddSpritesFieldValues(List<FieldValuePayload> fieldValues, SpritesPayload sprites)
  {
    fieldValues.Add(Forms.DefaultSprite, sprites.Default);
    fieldValues.Add(Forms.DefaultSpriteShiny, sprites.DefaultShiny);
    fieldValues.Add(Forms.AlternativeSprite, sprites.Alternative);
    fieldValues.Add(Forms.AlternativeSpriteShiny, sprites.AlternativeShiny);
  }

  private static void AddTypeFieldValues(List<FieldValuePayload> fieldValues, TypesPayload types)
  {
    fieldValues.Add(Forms.PrimaryType, SeedingSerializer.Serialize<PokemonType[]>([types.Primary]).ToLowerInvariant());

    if (types.Secondary.HasValue)
    {
      fieldValues.Add(Forms.SecondaryType, SeedingSerializer.Serialize<PokemonType[]>([types.Secondary.Value]).ToLowerInvariant());
    }
  }

  private static void AddYieldFieldValues(List<FieldValuePayload> fieldValues, YieldPayload yield)
  {
    fieldValues.Add(Forms.ExperienceYield, yield.Experience);

    fieldValues.Add(Forms.HPYield, yield.HP);
    fieldValues.Add(Forms.AttackYield, yield.Attack);
    fieldValues.Add(Forms.DefenseYield, yield.Defense);
    fieldValues.Add(Forms.SpecialAttackYield, yield.SpecialAttack);
    fieldValues.Add(Forms.SpecialDefenseYield, yield.SpecialDefense);
    fieldValues.Add(Forms.SpeedYield, yield.Speed);
  }
}
