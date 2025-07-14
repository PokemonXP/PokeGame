using Krakenar.Core;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace PokeGame.Core.Trainers;

public readonly struct TrainerId
{
  private const string EntityType = "Content";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public TrainerId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public TrainerId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public TrainerId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(TrainerId left, TrainerId right) => left.Equals(right);
  public static bool operator !=(TrainerId left, TrainerId right) => !left.Equals(right);

  public static TrainerId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TrainerId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
