using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Regions;

public readonly struct RegionId
{
  private const string EntityType = "Region";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RegionId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public RegionId(Guid value)
  {
    StreamId = IdHelper.Construct(EntityType, value);
  }
  public RegionId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static RegionId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => IdHelper.Deconstruct(StreamId, EntityType).Item1;

  public static bool operator ==(RegionId left, RegionId right) => left.Equals(right);
  public static bool operator !=(RegionId left, RegionId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RegionId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
