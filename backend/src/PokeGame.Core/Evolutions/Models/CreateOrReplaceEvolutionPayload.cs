using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Evolutions.Models;

public record CreateOrReplaceEvolutionPayload
{
  public string Source { get; set; } = string.Empty;
  public string Target { get; set; } = string.Empty;

  public EvolutionTrigger Trigger { get; set; }
  public string? Item { get; set; }

  public int Level { get; set; }
  public bool Friendship { get; set; }
  public PokemonGender? Gender { get; set; }
  public string? HeldItem { get; set; }
  public string? KnownMove { get; set; }
  public string? Location { get; set; }
  public TimeOfDay? TimeOfDay { get; set; }
}
