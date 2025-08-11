using MediatR;
using PokeGame.Core;
using PokeGame.Core.Forms.Models;
using PokeGame.ETL.Models;
using PokeGame.ETL.Settings;
using PokeGame.Tools.Shared;
using PokeGame.Tools.Shared.Models;
using Form = PokeGame.ETL.Models.Form;

namespace PokeGame.ETL.Tasks;

internal class ExtractFormsTask : EtlTask
{
  public override string? Description => "Extract Pokémon forms.";
}

internal class ExtractFormsTaskHandler : INotificationHandler<ExtractFormsTask>
{
  private const string DirectoryPath = "pokemon-form";
  private const string DataPath = "data/forms.csv";

  private readonly ILogger<ExtractFormsTaskHandler> _logger;
  private readonly ExtractionSettings _settings;

  public ExtractFormsTaskHandler(ILogger<ExtractFormsTaskHandler> logger, ExtractionSettings settings)
  {
    _logger = logger;
    _settings = settings;
  }

  public async Task Handle(ExtractFormsTask _, CancellationToken cancellationToken)
  {
    CsvManager csv = new([new SeedSpeciesPayload.Map(), new SeedFormPayload.Map()]);
    Dictionary<string, SeedFormPayload> forms = (await csv.ExtractAsync<SeedFormPayload>(DataPath, cancellationToken))
      .ToDictionary(x => x.UniqueName, x => x);
    _logger.LogInformation("Retrieved {Forms} forms from '{Path}'.", forms.Count, DataPath);

    IReadOnlyCollection<Form> extracted = await ExtractAsync(cancellationToken);
    foreach (Form data in extracted)
    {
      Variety? variety = await ExtractVarietyAsync(data, cancellationToken);
      if (variety is null)
      {
        continue;
      }
      Species? species = await ExtractSpeciesAsync(data, variety, cancellationToken);
      if (species is null)
      {
        continue;
      }

      FormTypesModel? types = ExtractTypes(data);
      if (types is null)
      {
        continue;
      }

      string uniqueName = data.UniqueName.Trim().ToLower();
      if (!forms.TryGetValue(uniqueName, out SeedFormPayload? form))
      {
        form = new SeedFormPayload
        {
          Id = Guid.NewGuid(),
          UniqueName = uniqueName
        };
        forms[form.UniqueName] = form;
      }
      form.Variety = variety.UniqueName;
      form.IsDefault = data.IsDefault;
      form.DisplayName ??= ExtractDisplayName(species);
      form.IsBattleOnly = data.IsBattleOnly;
      form.IsMega = data.IsMega;
      form.Height = variety.Height;
      form.Weight = variety.Weight;
      form.Types = types;
      form.Abilities = ExtractAbilities(variety);
      form.Yield.Experience = variety.BaseExperience;
      form.Sprites = ExtractSprites(variety);
      form.Url ??= ToUrl(form.DisplayName);

      foreach (VarietyStatistic item in variety.Statistics)
      {
        switch (item.Statistic.Name.ToLowerInvariant())
        {
          case "hp":
            form.BaseStatistics.HP = item.BaseValue;
            form.Yield.HP = item.EffortValue;
            break;
          case "attack":
            form.BaseStatistics.Attack = item.BaseValue;
            form.Yield.Attack = item.EffortValue;
            break;
          case "defense":
            form.BaseStatistics.Defense = item.BaseValue;
            form.Yield.Defense = item.EffortValue;
            break;
          case "special-attack":
            form.BaseStatistics.SpecialAttack = item.BaseValue;
            form.Yield.SpecialAttack = item.EffortValue;
            break;
          case "special-defense":
            form.BaseStatistics.SpecialDefense = item.BaseValue;
            form.Yield.SpecialDefense = item.EffortValue;
            break;
          case "speed":
            form.BaseStatistics.Speed = item.BaseValue;
            form.Yield.Speed = item.EffortValue;
            break;
        }
      }
    }

    await csv.SaveAsync(forms.Values, DataPath, cancellationToken);
    _logger.LogInformation("Saved {Forms} forms to '{Path}'.", forms.Count, DataPath);
  }

  private async Task<IReadOnlyCollection<Form>> ExtractAsync(CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_settings.Path, DirectoryPath);
    string[] paths = Directory.GetFiles(directory, searchPattern: "index.json", SearchOption.AllDirectories);
    List<Form> forms = new(capacity: paths.Length);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
      Form? form = null;
      try
      {
        form = JsonSerializer.Deserialize<Form>(json);
      }
      catch (Exception)
      {
      }
      if (form is not null && form.Id > 0 && form.Id < 10000 && form.IsDefault && !string.IsNullOrWhiteSpace(form.UniqueName))
      {
        forms.Add(form);
      }
    }
    return forms.OrderBy(x => x.Id).ToList().AsReadOnly();
  }

  private async Task<Species?> ExtractSpeciesAsync(Form form, Variety variety, CancellationToken cancellationToken)
  {
    string path = Path.Combine(_settings.Path, variety.Species.Url.Trim('/')[7..].Replace('/', '\\'), "index.json"); // NOTE(fpion): 7 is the length of "api/v2/"
    if (!File.Exists(path))
    {
      _logger.LogInformation("The form '{Form}' is being ignored because the species file does not exist: {Path}.", form.UniqueName, path);
      return null;
    }

    string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
    Species? species = JsonSerializer.Deserialize<Species>(json);
    if (species is null)
    {
      _logger.LogInformation("The form '{Form}' is being ignored because its species was not deserialized.", form.UniqueName);
    }
    return species;
  }
  private async Task<Variety?> ExtractVarietyAsync(Form form, CancellationToken cancellationToken)
  {
    string path = Path.Combine(_settings.Path, form.Variety.Url.Trim('/')[7..].Replace('/', '\\'), "index.json"); // NOTE(fpion): 7 is the length of "api/v2/"
    if (!File.Exists(path))
    {
      _logger.LogInformation("The form '{Form}' is being ignored because the variety file does not exist: {Path}.", form.UniqueName, path);
      return null;
    }

    string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
    Variety? variety = JsonSerializer.Deserialize<Variety>(json);
    if (variety is null)
    {
      _logger.LogInformation("The form '{Form}' is being ignored because its variety was not deserialized.", form.UniqueName);
    }
    return variety;
  }

  private string? ExtractDisplayName(Species species)
  {
    LocalizedName[] displayNames = species.DisplayNames
      .Where(x => x.Language.Name.Equals(_settings.Language, StringComparison.InvariantCultureIgnoreCase))
      .ToArray();
    if (displayNames.Length == 1)
    {
      return displayNames.Single().Name.Trim();
    }
    else if (displayNames.Length < 1)
    {
      _logger.LogWarning(
        "The species '{Species}' does not have a localized name for language '{Language}'.",
        species.UniqueName, _settings.Language);
    }
    else
    {
      _logger.LogWarning(
        "The species '{Species}' has multiple localized names ({Count}) for language '{Language}'.",
        species.UniqueName, displayNames.Length, _settings.Language);
    }
    return null;
  }

  private FormTypesModel? ExtractTypes(Form form)
  {
    PokemonType? primary = null;
    PokemonType? secondary = null;

    foreach (PokemonTypeSlot item in form.Types)
    {
      if (!Enum.TryParse(item.Type.Name, ignoreCase: true, out PokemonType type))
      {
        _logger.LogWarning("The form '{Form}' is being ignored because the Pokémon type '{Type}' is not valid.", form.UniqueName, item.Type.Name);
        return null;
      }

      switch (item.Slot)
      {
        case 1:
          primary = type;
          break;
        case 2:
          secondary = type;
          break;
      }
    }

    if (primary.HasValue)
    {
      return new FormTypesModel(primary.Value, secondary);
    }

    _logger.LogWarning("The form '{Form}' is being ignored because is does not have a primary type.", form.UniqueName);
    return null;
  }
  private static FormAbilitiesPayload ExtractAbilities(Variety variety)
  {
    FormAbilitiesPayload payload = new();
    foreach (PokemonAbilitySlot item in variety.Abilities)
    {
      if (item.IsHidden)
      {
        payload.Hidden = item.Ability.Name;
      }
      else
      {
        switch (item.Slot)
        {
          case 1:
            payload.Primary = item.Ability.Name;
            break;
          case 2:
            payload.Secondary = item.Ability.Name;
            break;
        }
      }
    }
    return payload;
  }
  private static SpritesModel ExtractSprites(Variety variety) => new()
  {
    Default = variety.Sprites.Other.Home.Default,
    Shiny = variety.Sprites.Other.Home.Shiny,
    Alternative = variety.Sprites.Other.Home.Alternative,
    AlternativeShiny = variety.Sprites.Other.Home.AlternativeShiny
  };

  private static string? ToUrl(string? displayName) => string.IsNullOrWhiteSpace(displayName) ? null
    : string.Format("https://bulbapedia.bulbagarden.net/wiki/{0}_(Pokémon)", displayName.Trim().Replace(' ', '_'));
}
