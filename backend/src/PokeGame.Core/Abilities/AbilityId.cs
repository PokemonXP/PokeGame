using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Abilities;

public readonly struct AbilityId
{
  private const string EntityType = "Ability";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public AbilityId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public AbilityId(Guid value)
  {
    StreamId = IdHelper.Construct(EntityType, value);
  }
  public AbilityId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static AbilityId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => IdHelper.Deconstruct(StreamId, EntityType).Item1;

  public static bool operator ==(AbilityId left, AbilityId right) => left.Equals(right);
  public static bool operator !=(AbilityId left, AbilityId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is AbilityId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
