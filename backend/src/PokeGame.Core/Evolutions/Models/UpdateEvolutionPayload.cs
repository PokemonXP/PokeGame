using Krakenar.Contracts;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Evolutions.Models;

public record UpdateEvolutionPayload
{
  public int? Level { get; set; }
  public bool? Friendship { get; set; }
  public Change<PokemonGender?>? Gender { get; set; }
  public Change<string>? HeldItem { get; set; }
  public Change<string>? KnownMove { get; set; }
  public Change<string>? Location { get; set; }
  public Change<TimeOfDay?>? TimeOfDay { get; set; }
}
