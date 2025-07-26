using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Evolutions;

public readonly struct EvolutionId
{
  private const string EntityType = "Evolution";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public EvolutionId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public EvolutionId(Guid value)
  {
    StreamId = IdHelper.Construct(EntityType, value);
  }
  public EvolutionId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static EvolutionId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => IdHelper.Deconstruct(StreamId, EntityType).Item1;

  public static bool operator ==(EvolutionId left, EvolutionId right) => left.Equals(right);
  public static bool operator !=(EvolutionId left, EvolutionId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is EvolutionId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
