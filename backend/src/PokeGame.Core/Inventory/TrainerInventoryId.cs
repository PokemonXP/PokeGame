using Logitar.EventSourcing;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Inventory;

public readonly struct TrainerInventoryId
{
  private const string EntityType = "Inventory";
  private const char Separator = '|';

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public TrainerId TrainerId => new(Value.Split(Separator).First());

  public TrainerInventoryId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public TrainerInventoryId(TrainerId trainerId)
  {
    StreamId = new StreamId(string.Join(Separator, trainerId, EntityType));
  }
  public TrainerInventoryId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static bool operator ==(TrainerInventoryId left, TrainerInventoryId right) => left.Equals(right);
  public static bool operator !=(TrainerInventoryId left, TrainerInventoryId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TrainerInventoryId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
