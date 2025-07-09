namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonSizeTests
{
  [Theory(DisplayName = "Categorize: it should return the expected Pokémon size category.")]
  [InlineData(0, PokemonSizeCategory.ExtraSmall)]
  [InlineData(10, PokemonSizeCategory.ExtraSmall)]
  [InlineData(16, PokemonSizeCategory.Small)]
  [InlineData(32, PokemonSizeCategory.Small)]
  [InlineData(48, PokemonSizeCategory.Medium)]
  [InlineData(100, PokemonSizeCategory.Medium)]
  [InlineData(208, PokemonSizeCategory.Large)]
  [InlineData(222, PokemonSizeCategory.Large)]
  [InlineData(240, PokemonSizeCategory.ExtraLarge)]
  [InlineData(255, PokemonSizeCategory.ExtraLarge)]
  public void Given_Size_When_Categorize_Then_CorrectCategory(byte height, PokemonSizeCategory expected)
  {
    PokemonSize size = new(height, weight: 0);
    Assert.Equal(expected, size.Category);
  }
}
