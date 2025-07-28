using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon.Events;

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

  public byte CurrentPowerPoints { get; private set; }
  public byte MaximumPowerPoints { get; private set; }
  public byte ReferencePowerPoints { get; private set; }

  public bool IsMastered { get; private set; }

  public int Level { get; private set; }
  public MoveLearningMethod Method { get; private set; }
  public ItemEntity? Item { get; private set; }
  public int? ItemId { get; private set; }
  public Guid? ItemUid { get; private set; }

  public string? Notes { get; private set; }

  public PokemonMoveEntity(PokemonEntity pokemon, MoveEntity move, PokemonMoveLearned @event)
  {
    Pokemon = pokemon;
    PokemonId = pokemon.PokemonId;
    PokemonUid = pokemon.Id;

    Move = move;
    MoveId = move.MoveId;
    MoveUid = move.Id;

    Position = @event.Position;

    CurrentPowerPoints = @event.Move.CurrentPowerPoints;
    MaximumPowerPoints = @event.Move.MaximumPowerPoints;
    ReferencePowerPoints = @event.Move.ReferencePowerPoints.Value;

    IsMastered = @event.Move.IsMastered;

    Level = @event.Move.Level.Value;
    Method = @event.Move.Method;

    Notes = @event.Move.Notes?.Value;
  }

  private PokemonMoveEntity()
  {
  }

  public void Remember(PokemonMoveRemembered @event)
  {
    Position = @event.Position;
  }

  public void Remove()
  {
    Position = null;
  }

  public void RestorePowerPoints()
  {
    CurrentPowerPoints = MaximumPowerPoints;
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

  public override bool Equals(object? obj) => obj is PokemonMoveEntity entity && entity.PokemonId == PokemonId && entity.MoveId == MoveId;
  public override int GetHashCode() => HashCode.Combine(PokemonId, MoveId);
  public override string ToString() => $"{GetType()} (PokemonId={PokemonId}, MoveId={MoveId})";
}
