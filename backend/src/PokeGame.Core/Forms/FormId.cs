using Logitar.EventSourcing;
using System.Diagnostics.CodeAnalysis;

namespace PokeGame.Core.Forms;

public readonly struct FormId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public FormId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public FormId(Guid value)
  {
    StreamId = new StreamId(value);
  }
  public FormId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static FormId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => StreamId.ToGuid();

  public static bool operator ==(FormId left, FormId right) => left.Equals(right);
  public static bool operator !=(FormId left, FormId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is FormId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
