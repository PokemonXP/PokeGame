using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Api.Models.Game;

public record OwnershipSummary
{
  public OwnershipKind Kind { get; set; }
  public int Level { get; set; }
  public string Location { get; set; }
  public DateTime MetOn { get; set; }
  public string? Description { get; set; }

  public OwnershipSummary() : this(string.Empty)
  {
  }

  public OwnershipSummary(string location)
  {
    Location = location;
  }

  public OwnershipSummary(OwnershipModel ownership)
  {
    Kind = ownership.Kind;
    Level = ownership.Level;
    Location = ownership.Location;
    MetOn = ownership.MetOn;
    Description = ownership.Description;
  }
}
