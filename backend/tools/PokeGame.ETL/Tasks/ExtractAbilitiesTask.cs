using MediatR;
using PokeGame.ETL.Models;
using PokeGame.ETL.Settings;
using PokeGame.Tools.Shared;
using PokeGame.Tools.Shared.Models;

namespace PokeGame.ETL.Tasks;

internal class ExtractAbilitiesTask : EtlTask
{
  public override string? Description => "Extract Pokémon abilities.";
}

internal class ExtractAbilitiesTaskHandler : INotificationHandler<ExtractAbilitiesTask>
{
  private const string DataPath = "data/abilities.csv";

  private readonly ILogger<ExtractAbilitiesTaskHandler> _logger;
  private readonly ExtractionSettings _settings;

  public ExtractAbilitiesTaskHandler(ILogger<ExtractAbilitiesTaskHandler> logger, ExtractionSettings settings)
  {
    _logger = logger;
    _settings = settings;
  }

  public async Task Handle(ExtractAbilitiesTask _, CancellationToken cancellationToken)
  {
    CsvManager csv = new([new SeedAbilityPayload.Map()]);
    Dictionary<string, SeedAbilityPayload> abilities = (await csv.ExtractAsync<SeedAbilityPayload>(DataPath, cancellationToken))
      .ToDictionary(x => x.UniqueName, x => x);
    _logger.LogInformation("Retrieved {Abilities} abilities from '{Path}'.", abilities.Count, DataPath);

    string directory = Path.Combine(_settings.Path, "ability");
    string[] paths = Directory.GetFiles(directory, searchPattern: "index.json", SearchOption.AllDirectories);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
      Ability? data = JsonSerializer.Deserialize<Ability>(json);
      if (data is not null && IsValid(data))
      {
        string uniqueName = data.UniqueName.Trim().ToLower();
        if (!abilities.TryGetValue(uniqueName, out SeedAbilityPayload? ability))
        {
          ability = new SeedAbilityPayload
          {
            Id = Guid.NewGuid(),
            UniqueName = uniqueName
          };
          abilities[ability.UniqueName] = ability;
        }
        ability.DisplayName ??= ExtractDisplayName(data);
        ability.Description ??= ExtractDescription(data);
        ability.Url ??= ToUrl(ability.DisplayName);
      }
    }

    await csv.SaveAsync(abilities.Values, DataPath, cancellationToken);
    _logger.LogInformation("Saved {Abilities} abilities to '{Path}'.", abilities.Count, DataPath);
  }

  private static bool IsValid(Ability ability) => ability.Id > 0 && !string.IsNullOrWhiteSpace(ability.UniqueName);

  private string? ExtractDisplayName(Ability ability)
  {
    LocalizedName[] displayNames = ability.DisplayNames
      .Where(x => x.Language.Name.Equals(_settings.Language, StringComparison.InvariantCultureIgnoreCase))
      .ToArray();
    if (displayNames.Length == 1)
    {
      return displayNames.Single().Name.Trim();
    }
    else if (displayNames.Length < 1)
    {
      _logger.LogWarning(
        "The ability '{Ability}' does not have a localized name for language '{Language}'.",
        ability.UniqueName, _settings.Language);
    }
    else
    {
      _logger.LogWarning(
        "The ability '{Ability}' has multiple localized names ({Count}) for language '{Language}'.",
        ability.UniqueName, displayNames.Length, _settings.Language);
    }
    return null;
  }

  private string? ExtractDescription(Ability ability)
  {
    FlavorText[] descriptions = ability.Descriptions
      .Where(x => x.Language.Name.Equals(_settings.Language, StringComparison.InvariantCultureIgnoreCase)
        && x.Version.Name.Equals(_settings.Version, StringComparison.InvariantCultureIgnoreCase))
      .ToArray();
    if (descriptions.Length == 1)
    {
      return descriptions.Single().Text.Trim();
    }
    else if (descriptions.Length < 1)
    {
      _logger.LogWarning(
        "The ability '{Ability}' does not have a flavor text for language '{Language}' and version '{Version}'.",
        ability.UniqueName, _settings.Language, _settings.Version);
    }
    else
    {
      _logger.LogWarning(
        "The ability '{Ability}' has multiple flavor texts ({Count}) for language '{Language}' and version '{Version}'.",
        ability.UniqueName, descriptions.Length, _settings.Language, _settings.Version);
    }
    return null;
  }

  private static string? ToUrl(string? displayName) => string.IsNullOrWhiteSpace(displayName) ? null
    : string.Format("https://bulbapedia.bulbagarden.net/wiki/{0}_(Ability)", displayName.Trim().Replace(' ', '_'));
}
