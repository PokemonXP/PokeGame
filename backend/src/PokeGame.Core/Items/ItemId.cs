using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Items;

public readonly struct ItemId
{
  private const string EntityType = "Item";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public ItemId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public ItemId(Guid value)
  {
    StreamId = IdHelper.Construct(EntityType, value);
  }
  public ItemId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static ItemId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => IdHelper.Deconstruct(StreamId, EntityType).Item1;

  public static bool operator ==(ItemId left, ItemId right) => left.Equals(right);
  public static bool operator !=(ItemId left, ItemId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ItemId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
