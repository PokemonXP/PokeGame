using MediatR;
using PokeGame.ETL.Models;
using PokeGame.ETL.Settings;
using System.Text.Json;

namespace PokeGame.ETL.Tasks;

internal class ExtractAbilitiesTask : EtlTask
{
  public override string? Description => "Extract Pokémon abilities.";
}

internal class ExtractAbilitiesTaskHandler : INotificationHandler<ExtractAbilitiesTask>
{
  private readonly ExtractionSettings _settings;

  public ExtractAbilitiesTaskHandler(ExtractionSettings settings)
  {
    _settings = settings;
  }

  public async Task Handle(ExtractAbilitiesTask _, CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_settings.Path, "ability");
    string[] paths = Directory.GetFiles(directory, searchPattern: "index.json", SearchOption.AllDirectories);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, _settings.Encoding, cancellationToken);
      Ability? ability = JsonSerializer.Deserialize<Ability>(json);
      if (ability is not null && ability.Id > 0)
      {
        string? displayName = ability.DisplayNames.SingleOrDefault(x => x.Language.Name == _settings.Language)?.Name;
        string? description = ability.Descriptions.SingleOrDefault(x => x.Language.Name == _settings.Language && x.Version.Name == _settings.Version)?.Text;

        // TODO(fpion): implement
      }
    }
  }
}
