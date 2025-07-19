using PokeGame.Core.Forms;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonStatisticsTests
{
  [Fact(DisplayName = "It should calculate the correct Pokémon statistics.")]
  public void Given_Arguments_When_ctor_Then_CorrectValues()
  {
    BaseStatistics baseStatistics = new(90, 93, 55, 70, 55, 55);
    IndividualValues individualValues = new(27, 27, 25, 22, 25, 26);
    EffortValues effortValues = new(82, 252, 88, 0, 88, 0);
    int level = 25;
    PokemonNature nature = PokemonNatures.Instance.Careful;
    PokemonStatistics statistics = new(baseStatistics, individualValues, effortValues, level, nature);

    Assert.Equal(91, statistics.HP);
    Assert.Equal(74, statistics.Attack);
    Assert.Equal(44, statistics.Defense);
    Assert.Equal(40, statistics.SpecialAttack);
    Assert.Equal(48, statistics.SpecialDefense);
    Assert.Equal(39, statistics.Speed);
  }
}
