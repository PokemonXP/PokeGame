using Logitar.EventSourcing;
using System.Diagnostics.CodeAnalysis;

namespace PokeGame.Core.Pokemons;

public readonly struct PokemonId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public PokemonId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public PokemonId(Guid value)
  {
    StreamId = new StreamId(value);
  }
  public PokemonId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static PokemonId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => StreamId.ToGuid();

  public static bool operator ==(PokemonId left, PokemonId right) => left.Equals(right);
  public static bool operator !=(PokemonId left, PokemonId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is PokemonId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
