using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Trainers;

public readonly struct TrainerId
{
  private const string EntityType = "Trainer";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public TrainerId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public TrainerId(Guid value)
  {
    StreamId = IdHelper.Construct(EntityType, value);
  }
  public TrainerId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static TrainerId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => IdHelper.Deconstruct(StreamId, EntityType).Item1;

  public static bool operator ==(TrainerId left, TrainerId right) => left.Equals(right);
  public static bool operator !=(TrainerId left, TrainerId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TrainerId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
