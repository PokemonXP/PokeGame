namespace PokeGame.Core.Pokemons.Models;

public record CreatePokemonPayload
{
  public Guid? Id { get; set; }

  public string Form { get; set; } = string.Empty;

  public string? UniqueName { get; set; }
  public string? Nickname { get; set; }
  public PokemonGender? Gender { get; set; }
  public bool IsShiny { get; set; }

  public PokemonType? TeraType { get; set; }
  public PokemonSizePayload? Size { get; set; }

  public int Experience { get; set; }

  public string? HeldItem { get; set; }

  public string? Sprite { get; set; }
  public string? Url { get; set; }
  public string? Notes { get; set; }
}
