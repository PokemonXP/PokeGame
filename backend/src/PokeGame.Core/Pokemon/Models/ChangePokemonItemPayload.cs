namespace PokeGame.Core.Pokemon.Models;

public record ChangePokemonItemPayload
{
  public string? HeldItem { get; set; }
}
