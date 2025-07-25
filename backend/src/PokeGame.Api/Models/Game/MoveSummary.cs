using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Api.Models.Game;

public record MoveSummary
{
  public PokemonType Type { get; set; }
  public MoveCategory Category { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public byte? Accuracy { get; set; }
  public byte? Power { get; set; }

  public byte CurrentPowerPoints { get; set; }
  public byte MaximumPowerPoints { get; set; }

  public MoveSummary() : this(string.Empty)
  {
  }

  public MoveSummary(string name)
  {
    Name = name;
  }

  public MoveSummary(PokemonMoveModel model)
  {
    MoveModel move = model.Move;

    Type = move.Type;
    Category = move.Category;

    Name = move.DisplayName ?? move.UniqueName;
    Description = move.Description;

    Accuracy = move.Accuracy;
    Power = move.Power;

    CurrentPowerPoints = model.PowerPoints.Current;
    MaximumPowerPoints = model.PowerPoints.Maximum;
  }
}
