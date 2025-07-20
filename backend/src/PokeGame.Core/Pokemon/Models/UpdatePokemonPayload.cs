using Krakenar.Contracts;

namespace PokeGame.Core.Pokemon.Models;

public record UpdatePokemonPayload
{
  public string? UniqueName { get; set; }
  public Change<string>? Nickname { get; set; }
  public bool? IsShiny { get; set; }
}
