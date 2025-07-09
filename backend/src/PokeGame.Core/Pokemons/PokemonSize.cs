namespace PokeGame.Core.Pokemons;

public record PokemonSize : IPokemonSize
{
  public byte Height { get; }
  public byte Weight { get; }
  public PokemonSizeCategory Category => Categorize(this);

  public PokemonSize()
  {
  }

  public PokemonSize(byte height, byte weight)
  {
    Height = height;
    Weight = weight;
  }

  public PokemonSize(IPokemonSize size) : this(size.Height, size.Weight)
  {
  }

  public static PokemonSizeCategory Categorize(IPokemonSize size)
  {
    if (size.Height <= 15)
    {
      return PokemonSizeCategory.ExtraSmall;
    }
    else if (size.Height <= 47)
    {
      return PokemonSizeCategory.Small;
    }
    else if (size.Height >= 240)
    {
      return PokemonSizeCategory.ExtraLarge;
    }
    else if (size.Height >= 208)
    {
      return PokemonSizeCategory.Large;
    }
    return PokemonSizeCategory.Medium;
  }
}
