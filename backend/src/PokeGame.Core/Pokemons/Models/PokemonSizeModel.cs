namespace PokeGame.Core.Pokemons.Models;

public record PokemonSizeModel : IPokemonSize
{
  public byte Height { get; set; }
  public byte Weight { get; set; }
  public PokemonSizeCategory Category => PokemonSize.Categorize(this);

  public PokemonSizeModel()
  {
  }

  public PokemonSizeModel(byte height, byte weight)
  {
    Height = height;
    Weight = weight;
  }

  public PokemonSizeModel(IPokemonSize size) : this(size.Height, size.Weight)
  {
  }
}
