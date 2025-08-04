using MediatR;
using PokeGame.ETL.Models;
using System.Text;
using System.Text.Json;

namespace PokeGame.ETL.Tasks;

internal class ExtractAbilitiesTask : EtlTask
{
  public override string? Description => "Extract Pokémon abilities.";
}

internal class ExtractAbilitiesTaskHandler : INotificationHandler<ExtractAbilitiesTask>
{
  public async Task Handle(ExtractAbilitiesTask _, CancellationToken cancellationToken)
  {
    string directory = "C:\\Users\\franc\\source\\repos\\PokeAPI\\api-data\\data\\api\\v2\\ability";
    Encoding encoding = Encoding.UTF8;

    string language = "en";
    string version = "scarlet-violet";

    string[] paths = Directory.GetFiles(directory, searchPattern: "index.json", SearchOption.AllDirectories);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, encoding, cancellationToken);
      Ability? ability = JsonSerializer.Deserialize<Ability>(json);
      if (ability is not null && ability.Id > 0)
      {
        string? displayName = ability.DisplayNames.SingleOrDefault(x => x.Language.Name == language)?.Name;
        string? description = ability.Descriptions.SingleOrDefault(x => x.Language.Name == language && x.Version.Name == version)?.Text;
      }
    }
  }
}
