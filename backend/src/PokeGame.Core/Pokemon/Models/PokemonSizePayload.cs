namespace PokeGame.Core.Pokemon.Models;

public record PokemonSizePayload : IPokemonSize
{
  public byte Height { get; set; }
  public byte Weight { get; set; }

  public PokemonSizePayload()
  {
  }

  public PokemonSizePayload(byte height, byte weight)
  {
    Height = height;
    Weight = weight;
  }

  public PokemonSizePayload(IPokemonSize size) : this(size.Height, size.Weight)
  {
  }
}
