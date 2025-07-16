using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Species;

public readonly struct SpeciesId
{
  private const string EntityType = "Species";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public SpeciesId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public SpeciesId(Guid value)
  {
    StreamId = IdHelper.Construct(EntityType, value);
  }
  public SpeciesId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static SpeciesId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => IdHelper.Deconstruct(StreamId, EntityType).Item1;

  public static bool operator ==(SpeciesId left, SpeciesId right) => left.Equals(right);
  public static bool operator !=(SpeciesId left, SpeciesId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is SpeciesId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
