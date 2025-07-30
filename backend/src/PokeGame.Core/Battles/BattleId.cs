using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Battles;

public readonly struct BattleId
{
  private const string EntityType = "Battle";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public BattleId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public BattleId(Guid value)
  {
    StreamId = IdHelper.Construct(EntityType, value);
  }
  public BattleId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static BattleId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => IdHelper.Deconstruct(StreamId, EntityType).Item1;

  public static bool operator ==(BattleId left, BattleId right) => left.Equals(right);
  public static bool operator !=(BattleId left, BattleId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is BattleId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
