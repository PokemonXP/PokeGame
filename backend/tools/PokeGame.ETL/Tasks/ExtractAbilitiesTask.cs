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
    string directory = Path.Combine(_settings.Path, "ability");
    string[] paths = Directory.GetFiles(directory, searchPattern: "index.json", SearchOption.AllDirectories);

    CsvManager csv = new([new SeedAbilityPayload.Map()]);
    Dictionary<string, SeedAbilityPayload> abilities = (await csv.ExtractAsync<SeedAbilityPayload>(DataPath, cancellationToken))
      .ToDictionary(x => x.UniqueName, x => x);
    _logger.LogInformation("Retrieved {Abilities} abilities from '{Path}'.", abilities.Count, DataPath);

    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
      Ability? data = JsonSerializer.Deserialize<Ability>(json);
      if (data is not null && data.Id > 0 && !string.IsNullOrWhiteSpace(data.UniqueName))
      {
        string uniqueName = data.UniqueName.Trim().ToLower();
        string? displayName = data.DisplayNames.SingleOrDefault(x => x.Language.Name == _settings.Language)?.Name;
        string? url = null;
        if (string.IsNullOrWhiteSpace(displayName))
        {
          _logger.LogWarning("The ability '{Ability}' does not have a localized name for language '{Language}'.", uniqueName, _settings.Language);
        }
        else
        {
          url = string.Format("https://bulbapedia.bulbagarden.net/wiki/{0}_(Ability)", displayName.Replace(' ', '_'));
        }

        string? description = data.Descriptions.SingleOrDefault(x => x.Language.Name == _settings.Language && x.Version.Name == _settings.Version)?.Text;
        if (string.IsNullOrWhiteSpace(description))
        {
          _logger.LogWarning("The ability '{Ability}' does not have a flavor text name for language '{Language}' and version '{Version}'.",
            displayName ?? uniqueName, _settings.Language, _settings.Version);
        }

        if (!abilities.TryGetValue(uniqueName, out SeedAbilityPayload? ability))
        {
          ability = new SeedAbilityPayload
          {
            Id = Guid.NewGuid(),
            UniqueName = uniqueName
          };
          abilities[ability.UniqueName] = ability;
        }
        ability.DisplayName ??= displayName;
        ability.Description ??= description;
        ability.Url = url;
      }
    }

    await csv.SaveAsync(abilities.Values, DataPath, cancellationToken);
    _logger.LogInformation("Saved {Abilities} abilities to '{Path}'.", abilities.Count, DataPath);
  }
}
