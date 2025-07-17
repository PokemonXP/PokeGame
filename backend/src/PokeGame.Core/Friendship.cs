namespace PokeGame.Core;

public record Friendship
{
  public byte Value { get; }

  public Friendship() : this(value: 0)
  {
  }

  public Friendship(byte value)
  {
    Value = value;
  }

  public override string ToString() => Value.ToString();
}
