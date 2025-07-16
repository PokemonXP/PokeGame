namespace PokeGame.Core.Pokemons.Models;

public record CreatePokemonPayload
{
  public Guid? Id { get; set; }

  public string Form { get; set; } = string.Empty;

  public string? UniqueName { get; set; }
}
