using Krakenar.Core;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace PokeGame.Core.Forms;

public readonly struct FormId
{
  private const string EntityType = "Content";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public FormId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public FormId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public FormId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(FormId left, FormId right) => left.Equals(right);
  public static bool operator !=(FormId left, FormId right) => !left.Equals(right);

  public static FormId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is FormId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
