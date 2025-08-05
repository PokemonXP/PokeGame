using MediatR;
using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.ETL.Models;
using PokeGame.ETL.Settings;
using PokeGame.Tools.Shared;
using PokeGame.Tools.Shared.Models;
using Move = PokeGame.ETL.Models.Move;

namespace PokeGame.ETL.Tasks;

internal class ExtractMovesTask : EtlTask
{
  public override string? Description => "Extract Pokémon moves.";
}

internal class ExtractMovesTaskHandler : INotificationHandler<ExtractMovesTask>
{
  private const string DirectoryPath = "move";
  private const string DataPath = "data/moves.csv";

  private readonly ILogger<ExtractMovesTask> _logger;
  private readonly ExtractionSettings _settings;

  public ExtractMovesTaskHandler(ILogger<ExtractMovesTask> logger, ExtractionSettings settings)
  {
    _logger = logger;
    _settings = settings;
  }

  public async Task Handle(ExtractMovesTask _, CancellationToken cancellationToken)
  {
    CsvManager csv = new([new SeedMovePayload.Map()]);
    Dictionary<string, SeedMovePayload> moves = (await csv.ExtractAsync<SeedMovePayload>(DataPath, cancellationToken))
      .ToDictionary(x => x.UniqueName, x => x);
    _logger.LogInformation("Retrieved {Moves} moves from '{Path}'.", moves.Count, DataPath);

    IReadOnlyCollection<Move> extracted = await ExtractAsync(cancellationToken);
    foreach (Move data in extracted)
    {
      string uniqueName = data.UniqueName.Trim().ToLower();
      if (!Enum.TryParse(data.Type.Name.Trim(), ignoreCase: true, out PokemonType type))
      {
        _logger.LogWarning("The move '{Move}' is being ignored because its type '{Type}' cannot be parsed.", uniqueName, data.Type.Name);
        continue;
      }
      if (!Enum.TryParse(data.Category.Name.Trim(), ignoreCase: true, out MoveCategory category))
      {
        _logger.LogWarning("The move '{Move}' is being ignored because its category '{Category}' cannot be parsed.", uniqueName, data.Category.Name);
        continue;
      }
      if (!data.PowerPoints.HasValue)
      {
        _logger.LogWarning("The move '{Move}' is being ignored because it does not have Power Points (PP).", uniqueName);
        continue;
      }

      if (!moves.TryGetValue(uniqueName, out SeedMovePayload? move))
      {
        move = new SeedMovePayload
        {
          Id = Guid.NewGuid(),
          UniqueName = uniqueName
        };
        moves[move.UniqueName] = move;
      }
      move.Type = type;
      move.Category = category;
      move.DisplayName = string.IsNullOrWhiteSpace(move.DisplayName) ? ExtractDisplayName(data) : move.DisplayName;
      move.Description = string.IsNullOrWhiteSpace(move.Description) ? ExtractDescription(data) : move.Description;
      move.Accuracy = data.Accuracy <= 0 ? null : data.Accuracy;
      move.Power = data.Power <= 0 ? null : data.Power;
      move.PowerPoints = data.PowerPoints.Value;
      move.Url = string.IsNullOrWhiteSpace(move.Url) ? ToUrl(move.DisplayName) : move.Url;
    }

    await csv.SaveAsync(moves.Values, DataPath, cancellationToken);
    _logger.LogInformation("Saved {Moves} moves to '{Path}'.", moves.Count, DataPath);
  }

  private async Task<IReadOnlyCollection<Move>> ExtractAsync(CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_settings.Path, DirectoryPath);
    string[] paths = Directory.GetFiles(directory, searchPattern: "index.json", SearchOption.AllDirectories);
    List<Move> moves = new(capacity: paths.Length);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
      Move? move = null;
      try
      {
        move = JsonSerializer.Deserialize<Move>(json);
      }
      catch (Exception)
      {
      }
      if (move is not null && move.Id > 0 && move.Id < 10000 && !string.IsNullOrWhiteSpace(move.UniqueName))
      {
        moves.Add(move);
      }
    }
    return moves.OrderBy(x => x.Id).ToList().AsReadOnly();
  }

  private string? ExtractDisplayName(Move move)
  {
    LocalizedName[] displayNames = move.DisplayNames
      .Where(x => x.Language.Name.Equals(_settings.Language, StringComparison.InvariantCultureIgnoreCase))
      .ToArray();
    if (displayNames.Length == 1)
    {
      return displayNames.Single().Name.Trim();
    }
    else if (displayNames.Length < 1)
    {
      _logger.LogWarning(
        "The move '{Move}' does not have a localized name for language '{Language}'.",
        move.UniqueName, _settings.Language);
    }
    else
    {
      _logger.LogWarning(
        "The move '{Move}' has multiple localized names ({Count}) for language '{Language}'.",
        move.UniqueName, displayNames.Length, _settings.Language);
    }
    return null;
  }

  private string? ExtractDescription(Move move)
  {
    FlavorText[] descriptions = move.Descriptions
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
        "The move '{Move}' does not have a flavor text for language '{Language}' and version '{Version}'.",
        move.UniqueName, _settings.Language, _settings.Version);
    }
    else
    {
      _logger.LogWarning(
        "The move '{Move}' has multiple flavor texts ({Count}) for language '{Language}' and version '{Version}'.",
        move.UniqueName, descriptions.Length, _settings.Language, _settings.Version);
    }
    return null;
  }

  private static string? ToUrl(string? displayName) => string.IsNullOrWhiteSpace(displayName) ? null
    : string.Format("https://bulbapedia.bulbagarden.net/wiki/{0}_(move)", displayName.Trim().Replace(' ', '_'));
}
