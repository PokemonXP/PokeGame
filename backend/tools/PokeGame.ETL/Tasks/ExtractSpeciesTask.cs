using MediatR;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Models;
using PokeGame.ETL.Models;
using PokeGame.ETL.Settings;
using PokeGame.Tools.Shared;
using PokeGame.Tools.Shared.Models;
using Species = PokeGame.ETL.Models.Species;

namespace PokeGame.ETL.Tasks;

internal class ExtractSpeciesTask : EtlTask
{
  public override string? Description => "Extract Pokémon species.";
}

internal class ExtractSpeciesTaskHandler : INotificationHandler<ExtractSpeciesTask>
{
  private const string DataPath = "data/species.csv";

  private readonly ILogger<ExtractSpeciesTask> _logger;
  private readonly ExtractionSettings _settings;

  public ExtractSpeciesTaskHandler(ILogger<ExtractSpeciesTask> logger, ExtractionSettings settings)
  {
    _logger = logger;
    _settings = settings;
  }

  public async Task Handle(ExtractSpeciesTask _, CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_settings.Path, "pokemon-species");
    string[] paths = Directory.GetFiles(directory, searchPattern: "index.json", SearchOption.AllDirectories);

    CsvManager csv = new([new SeedSpeciesPayload.Map()]);
    Dictionary<string, SeedSpeciesPayload> speciesByName = (await csv.ExtractAsync<SeedSpeciesPayload>(DataPath, cancellationToken))
      .ToDictionary(x => x.UniqueName, x => x);
    _logger.LogInformation("Retrieved {Species} species from '{Path}'.", speciesByName.Count, DataPath);

    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
      Species? data = JsonSerializer.Deserialize<Species>(json);
      if (data is not null && IsValid(data))
      {
        string uniqueName = data.UniqueName.Trim().ToLower();
        int? number = ExtractNumber(data);
        PokemonCategory? category = ExtractCategory(data);
        GrowthRate? growthRate = ExtractGrowthRate(data);
        EggGroupsModel? eggGroups = ExtractEggGroups(data);
        if (!number.HasValue || !category.HasValue || !growthRate.HasValue || eggGroups is null)
        {
          continue;
        }

        if (!speciesByName.TryGetValue(uniqueName, out SeedSpeciesPayload? species))
        {
          species = new SeedSpeciesPayload
          {
            Id = Guid.NewGuid(),
            UniqueName = uniqueName
          };
          speciesByName[species.UniqueName] = species;
        }
        species.Number = number.Value;
        species.Category = category.Value;
        species.DisplayName = string.IsNullOrWhiteSpace(species.DisplayName) ? ExtractDisplayName(data) : species.DisplayName;
        species.BaseFriendship = data.BaseFriendship;
        species.CatchRate = data.CatchRate;
        species.GrowthRate = growthRate.Value;
        species.EggCycles = data.EggCycles;
        species.EggGroups = eggGroups;
        species.Url = string.IsNullOrWhiteSpace(species.Url) ? ToUrl(species.DisplayName) : species.Url;
      }
    }

    await csv.SaveAsync(speciesByName.Values, DataPath, cancellationToken);
    _logger.LogInformation("Saved {Species} species to '{Path}'.", speciesByName.Count, DataPath);
  }

  private static bool IsValid(Species species) => species.Id > 0 && !string.IsNullOrWhiteSpace(species.UniqueName);

  private int? ExtractNumber(Species species)
  {
    PokedexNumber[] numbers = species.PokedexNumbers.Where(x => x.Pokedex.Name.Equals("national", StringComparison.InvariantCultureIgnoreCase)).ToArray();
    if (numbers.Length == 1)
    {
      return numbers.Single().Number;
    }
    else if (numbers.Length < 1)
    {
      _logger.LogWarning("The species '{Species}' is being ignored because it does not have a national Pokédex number.", species.UniqueName);
    }
    else
    {
      _logger.LogWarning("The species '{Species}' is being ignored because it has multiple national Pokédex numbers ({Numbers}).", species.UniqueName, numbers.Length);
    }
    return null;
  }

  private PokemonCategory? ExtractCategory(Species species)
  {
    List<PokemonCategory> categories = new(capacity: 3);
    if (species.IsBaby)
    {
      categories.Add(PokemonCategory.Baby);
    }
    if (species.IsLegendary)
    {
      categories.Add(PokemonCategory.Legendary);
    }
    if (species.IsMythical)
    {
      categories.Add(PokemonCategory.Mythical);
    }

    if (categories.Count < 1)
    {
      return PokemonCategory.Standard;
    }
    else if (categories.Count == 1)
    {
      return categories.Single();
    }

    _logger.LogWarning("The species '{Species}' is being ignored because it has multiple categories: {Categories}.",
      species.UniqueName, string.Join(", ", categories));
    return null;
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

  private GrowthRate? ExtractGrowthRate(Species species)
  {
    switch (species.GrowthRate.Name.Trim().ToLower())
    {
      case "fast":
        return GrowthRate.Fast;
      case "fast-then-very-slow":
        return GrowthRate.Fluctuating;
      case "medium":
        return GrowthRate.MediumFast;
      case "medium-slow":
        return GrowthRate.MediumSlow;
      case "slow":
        return GrowthRate.Slow;
      case "slow-then-very-fast":
        return GrowthRate.Erratic;
    }

    _logger.LogWarning("The species '{Species}' is being ignored because it does not have a valid growth rate: {Categories}.",
      species.UniqueName, species.GrowthRate.Name);
    return null;
  }

  private EggGroupsModel? ExtractEggGroups(Species species)
  {
    if (species.EggGroups.Count < 1)
    {
      _logger.LogWarning("The species '{Species}' is being ignored because it does not have any egg group.", species.UniqueName);
      return null;
    }
    if (species.EggGroups.Count > 2)
    {
      _logger.LogWarning("The species '{Species}' is being ignored because it has too many egg groups ({Groups}).", species.UniqueName, species.EggGroups.Count);
      return null;
    }

    EggGroup? primary = ExtractEggGroup(species.EggGroups.First());
    if (!primary.HasValue)
    {
      _logger.LogWarning("The species '{Species}' is being ignored because it does not have a valid primary egg group: '{Group}'.",
        species.UniqueName, species.EggGroups.First().Name);
      return null;
    }
    EggGroup? secondary = null;
    if (species.EggGroups.Count == 2)
    {
      secondary = ExtractEggGroup(species.EggGroups.Last());
      if (!secondary.HasValue)
      {
        _logger.LogWarning("The species '{Species}' is being ignored because it does not have a valid secondary egg group: '{Group}'.",
          species.UniqueName, species.EggGroups.Last().Name);
        return null;
      }
    }

    return new EggGroupsModel(primary.Value, secondary);
  }
  private static EggGroup? ExtractEggGroup(NamedResource group)
  {
    switch (group.Name.Trim().ToLower())
    {
      case "ground":
        return EggGroup.Field;
      case "humanshape":
        return EggGroup.HumanLike;
      case "indeterminate":
        return EggGroup.Amorphous;
      case "no-eggs":
        return EggGroup.NoEggsDiscovered;
      case "plant":
        return EggGroup.Grass;
    }

    if (Enum.TryParse(group.Name, ignoreCase: true, out EggGroup result))
    {
      return result;
    }

    return null;
  }

  private static string? ToUrl(string? displayName) => string.IsNullOrWhiteSpace(displayName) ? null
    : string.Format("https://bulbapedia.bulbagarden.net/wiki/{0}_(Pokémon)", displayName.Trim().Replace(' ', '_'));
}
