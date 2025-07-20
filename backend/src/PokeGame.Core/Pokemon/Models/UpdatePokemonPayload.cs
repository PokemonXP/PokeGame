using Krakenar.Contracts;

namespace PokeGame.Core.Pokemon.Models;

public record UpdatePokemonPayload
{
  public string? UniqueName { get; set; }
  public Change<string>? Nickname { get; set; }
  public bool? IsShiny { get; set; }

  public Change<string>? Sprite { get; set; }
  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }
}
