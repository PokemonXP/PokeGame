namespace PokeGame.Core.Battles.Models;

[Trait(Traits.Category, Categories.Unit)]
public class CriticalModelTests
{
  [Theory(DisplayName = "It should assign the correct chance given valid stage.")]
  [InlineData(0, 5)]
  [InlineData(1, 10)]
  [InlineData(2, 25)]
  [InlineData(3, 50)]
  [InlineData(4, 100)]
  public void Given_ValidStage_When_ctor_Then_CorrectChance(int stages, int chance)
  {
    CriticalModel critical = new(stages);
    Assert.Equal(stages, critical.Stages);
    Assert.Equal(chance, critical.Chance);
  }

  [Theory(DisplayName = "It should assign the maximum chance given stages too high.")]
  [InlineData(5)]
  public void Given_MaximumExceeded_When_ctor_Then_MaximumChance(int stages)
  {
    Assert.True(stages > StatisticChanges.MaximumCritical);

    CriticalModel model = new(stages);
    Assert.Equal(100, model.Chance);
  }

  [Theory(DisplayName = "It should assign the minimum chance given stages too low.")]
  [InlineData(-10)]
  public void Given_MinimumExceeded_When_ctor_Then_MinimumChance(int stages)
  {
    Assert.True(stages < StatisticChanges.MinimumCritical);

    CriticalModel model = new(stages);
    Assert.Equal(5, model.Chance);
  }
}
