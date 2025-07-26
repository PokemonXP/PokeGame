namespace PokeGame.Core.Pokemon.Models;

public record ChangePokemonFormPayload
{
  public string Form { get; set; } = string.Empty;
}
