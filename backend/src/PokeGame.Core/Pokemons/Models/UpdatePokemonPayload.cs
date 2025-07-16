using Krakenar.Contracts;

namespace PokeGame.Core.Pokemons.Models;

public record UpdatePokemonPayload
{
  public string? UniqueName { get; set; }
  public Change<string>? Nickname { get; set; }
  public PokemonGender? Gender { get; set; }

  public int? Vitality { get; set; }
  public int? Stamina { get; set; }
  public Change<StatusCondition?>? StatusCondition { get; set; }

  public byte? Friendship { get; set; }

  public Change<string>? Sprite { get; set; }
  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }
}
