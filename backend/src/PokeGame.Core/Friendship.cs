namespace PokeGame.Core;

public record Friendship
{
  public byte Value { get; }

  public Friendship(byte value)
  {
    Value = value;
  }

  public override string ToString() => Value.ToString();
}
