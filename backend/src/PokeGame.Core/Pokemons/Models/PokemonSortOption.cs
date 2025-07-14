using Krakenar.Contracts.Search;

namespace PokeGame.Core.Pokemons.Models;

public record PokemonSortOption : SortOption
{
  public new PokemonSort Field
  {
    get => Enum.Parse<PokemonSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public PokemonSortOption() : this(PokemonSort.UniqueName)
  {
  }

  public PokemonSortOption(PokemonSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
