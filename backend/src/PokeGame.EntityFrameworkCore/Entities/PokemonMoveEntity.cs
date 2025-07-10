using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons.Events;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class PokemonMoveEntity
{
  public PokemonEntity? Pokemon { get; private set; }
  public int PokemonId { get; private set; }
  public Guid PokemonUid { get; private set; }

  public MoveEntity? Move { get; private set; }
  public int MoveId { get; private set; }
  public Guid MoveUid { get; private set; }

  public int? Position { get; private set; }

  public int CurrentPowerPoints { get; private set; }
  public int MaximumPowerPoints { get; private set; }
  public int ReferencePowerPoints { get; private set; }

  public bool IsMastered { get; private set; }

  public int Level { get; private set; }
  public bool TechnicalMachine { get; private set; }

  public PokemonMoveEntity(PokemonEntity pokemon, MoveEntity move, PokemonMoveLearned @event)
    : this(pokemon, move, @event.Position, @event.PowerPoints)
  {
  }
  public PokemonMoveEntity(PokemonEntity pokemon, MoveEntity move, PokemonTechnicalMachineUsed @event)
    : this(pokemon, move, @event.Position, @event.PowerPoints)
  {
    TechnicalMachine = true;
  }
  private PokemonMoveEntity(PokemonEntity pokemon, MoveEntity move, int? position, PowerPoints powerPoints)
  {
    Pokemon = pokemon;
    PokemonId = pokemon.PokemonId;
    PokemonUid = pokemon.Id;

    Move = move;
    MoveId = move.MoveId;
    MoveUid = move.Id;

    Position = position ?? pokemon.Moves.Count;

    int powerPointsValue = powerPoints.Value;
    CurrentPowerPoints = powerPointsValue;
    MaximumPowerPoints = powerPointsValue;
    ReferencePowerPoints = powerPointsValue;

    Level = pokemon.Level;
  }

  private PokemonMoveEntity()
  {
  }

  public void Master(PokemonMoveMastered _)
  {
    IsMastered = true;
  }

  public void Relearn(PokemonMoveRelearned @event)
  {
    Position = @event.Position;
  }

  public void Remove()
  {
    Position = null;
  }

  public void Switch(PokemonMoveEntity other)
  {
    if (Position.HasValue && other.Position.HasValue)
    {
      int position = Position.Value;
      Position = other.Position;
      other.Position = position;
    }
  }
}
