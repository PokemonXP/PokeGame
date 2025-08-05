using Logitar;
using MediatR;
using PokeGame.Core.Varieties;
using PokeGame.ETL.Models;
using PokeGame.ETL.Settings;
using PokeGame.Tools.Shared;
using PokeGame.Tools.Shared.Models;
using Genus = PokeGame.ETL.Models.Genus;
using Variety = PokeGame.ETL.Models.Variety;

namespace PokeGame.ETL.Tasks;

internal class ExtractVarietiesTask : EtlTask
{
  public override string? Description => "Extract Pokémon varieties.";
}

internal class ExtractVarietiesTaskHandler : INotificationHandler<ExtractVarietiesTask>
{
  private const string DirectoryPath = "pokemon";
  private const string DataPath = "data/varieties.csv";

  private readonly ILogger<ExtractVarietiesTaskHandler> _logger;
  private readonly ExtractionSettings _settings;

  public ExtractVarietiesTaskHandler(ILogger<ExtractVarietiesTaskHandler> logger, ExtractionSettings settings)
  {
    _logger = logger;
    _settings = settings;
  }

  public async Task Handle(ExtractVarietiesTask _, CancellationToken cancellationToken)
  {
    CsvManager csv = new([new SeedSpeciesPayload.Map(), new SeedVarietyPayload.Map()]);
    Dictionary<string, SeedVarietyPayload> varieties = (await csv.ExtractAsync<SeedVarietyPayload>(DataPath, cancellationToken))
      .ToDictionary(x => x.UniqueName, x => x);
    _logger.LogInformation("Retrieved {Varieties} varieties from '{Path}'.", varieties.Count, DataPath);

    IReadOnlyCollection<Variety> extracted = await ExtractAsync(cancellationToken);
    foreach (Variety data in extracted)
    {
      Species? species = await ExtractSpeciesAsync(data, cancellationToken);
      if (species is null)
      {
        continue;
      }

      string uniqueName = data.UniqueName.Trim().ToLower();
      if (!varieties.TryGetValue(uniqueName, out SeedVarietyPayload? variety))
      {
        variety = new SeedVarietyPayload
        {
          Id = Guid.NewGuid(),
          UniqueName = uniqueName
        };
        varieties[variety.UniqueName] = variety;
      }
      variety.Species = species.UniqueName;
      variety.IsDefault = data.IsDefault;
      variety.DisplayName ??= ExtractDisplayName(species);
      variety.Genus ??= ExtractGenus(species);
      variety.GenderRatio = species.GenderRatio < 0 ? null : (GenderRatio.MaximumValue - species.GenderRatio);
      variety.CanChangeForm = species.CanChangeForm;
      variety.Url ??= ToUrl(variety.DisplayName);
    }

    await csv.SaveAsync(varieties.Values, DataPath, cancellationToken);
    _logger.LogInformation("Saved {Varieties} varieties to '{Path}'.", varieties.Count, DataPath);
  }

  private async Task<IReadOnlyCollection<Variety>> ExtractAsync(CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_settings.Path, DirectoryPath);
    string[] paths = Directory.GetFiles(directory, searchPattern: "index.json", SearchOption.AllDirectories);
    List<Variety> varieties = new(capacity: paths.Length);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
      Variety? variety = null;
      try
      {
        variety = JsonSerializer.Deserialize<Variety>(json);
      }
      catch (Exception)
      {
      }
      if (variety is not null && variety.Id > 0 && variety.Id < 10000 && variety.IsDefault && !string.IsNullOrWhiteSpace(variety.UniqueName))
      {
        varieties.Add(variety);
      }
    }
    return varieties.OrderBy(x => x.Id).ToList().AsReadOnly();
  }

  private async Task<Species?> ExtractSpeciesAsync(Variety variety, CancellationToken cancellationToken)
  {
    string path = Path.Combine(_settings.Path, variety.Species.Url.Trim('/')[7..].Replace('/', '\\'), "index.json"); // NOTE(fpion): 7 is the length of "api/v2/"
    if (!File.Exists(path))
    {
      _logger.LogInformation("The variety '{Variety}' is being ignored because the species file does not exist: {Path}.", variety.UniqueName, path);
      return null;
    }

    string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
    Species? species = JsonSerializer.Deserialize<Species>(json);
    if (species is null)
    {
      _logger.LogInformation("The variety '{Variety}' is being ignored because its species was not deserialized.", variety.UniqueName);
    }
    return species;
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

  private string? ExtractGenus(Species species)
  {
    Genus[] genera = species.Genera
      .Where(x => x.Language.Name.Equals(_settings.Language, StringComparison.InvariantCultureIgnoreCase))
      .ToArray();
    if (genera.Length == 1)
    {
      return genera.Single().Value.Trim().Remove(" Pokémon");
    }
    else if (genera.Length < 1)
    {
      _logger.LogWarning(
        "The species '{Species}' does not have a genus for language '{Language}'.",
        species.UniqueName, _settings.Language);
    }
    else
    {
      _logger.LogWarning(
        "The species '{Species}' has multiple genera ({Count}) for language '{Language}'.",
        species.UniqueName, genera.Length, _settings.Language);
    }
    return null;
  }

  private static string? ToUrl(string? displayName) => string.IsNullOrWhiteSpace(displayName) ? null
    : string.Format("https://bulbapedia.bulbagarden.net/wiki/{0}_(Pokémon)", displayName.Trim().Replace(' ', '_'));
}
