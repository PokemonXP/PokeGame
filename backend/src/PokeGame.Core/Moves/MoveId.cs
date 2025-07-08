using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Moves;

public readonly struct MoveId
{
  private const string EntityType = "Move";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public MoveId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public MoveId(Guid value)
  {
    StreamId = IdHelper.Construct(EntityType, value);
  }
  public MoveId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static MoveId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => IdHelper.Deconstruct(StreamId, EntityType).Item1;

  public static bool operator ==(MoveId left, MoveId right) => left.Equals(right);
  public static bool operator !=(MoveId left, MoveId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is MoveId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
