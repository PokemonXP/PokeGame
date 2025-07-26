using PokeGame.Core.Forms.Models;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Pokemon;
using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Evolutions.Models;

public class EvolutionModel : AggregateModel
{
  public FormModel Source { get; set; } = new();
  public FormModel Target { get; set; } = new();

  public EvolutionTrigger Trigger { get; set; }
  public ItemModel? Item { get; set; }

  public int Level { get; set; }
  public bool Friendship { get; set; }
  public PokemonGender? Gender { get; set; }
  public ItemModel? HeldItem { get; set; }
  public MoveModel? KnownMove { get; set; }
  public string? Location { get; set; }
  public TimeOfDay? TimeOfDay { get; set; }
}
