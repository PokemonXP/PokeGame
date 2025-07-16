using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Varieties;

public readonly struct VarietyId
{
  private const string EntityType = "Variety";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public VarietyId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public VarietyId(Guid value)
  {
    StreamId = IdHelper.Construct(EntityType, value);
  }
  public VarietyId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static VarietyId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => IdHelper.Deconstruct(StreamId, EntityType).Item1;

  public static bool operator ==(VarietyId left, VarietyId right) => left.Equals(right);
  public static bool operator !=(VarietyId left, VarietyId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is VarietyId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
