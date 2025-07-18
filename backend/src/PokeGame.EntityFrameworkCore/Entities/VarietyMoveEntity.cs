using PokeGame.Core.Varieties.Events;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class VarietyMoveEntity
{
  public VarietyEntity? Variety { get; private set; }
  public int VarietyId { get; private set; }
  public Guid VarietyUid { get; private set; }

  public MoveEntity? Move { get; private set; }
  public int MoveId { get; private set; }
  public Guid MoveUid { get; private set; }

  public int Level { get; private set; }

  public VarietyMoveEntity(VarietyEntity variety, MoveEntity move, VarietyEvolutionMoveChanged @event) : this(variety, move)
  {
    Update(@event);
  }
  public VarietyMoveEntity(VarietyEntity variety, MoveEntity move, VarietyLevelMoveChanged @event) : this(variety, move)
  {
    Update(@event);
  }
  private VarietyMoveEntity(VarietyEntity variety, MoveEntity move)
  {
    Variety = variety;
    VarietyId = variety.VarietyId;
    VarietyUid = variety.Id;

    Move = move;
    MoveId = move.MoveId;
    MoveUid = move.Id;
  }

  private VarietyMoveEntity()
  {
  }

  public void Update(VarietyEvolutionMoveChanged _)
  {
    Level = 0;
  }
  public void Update(VarietyLevelMoveChanged @event)
  {
    Level = @event.Level.Value;
  }

  public override bool Equals(object? obj) => obj is VarietyMoveEntity entity && entity.VarietyId == VarietyId && entity.MoveId == MoveId;
  public override int GetHashCode() => HashCode.Combine(VarietyId, MoveId);
  public override string ToString() => $"{GetType()} (VarietyId={VarietyId}, MoveId={MoveId})";
}
