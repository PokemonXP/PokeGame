namespace PokeGame.Core.Battles.Models;

[Trait(Traits.Category, Categories.Unit)]
public class BattleStatisticModelTests
{
  [Theory(DisplayName = "It should augment the value when stages are positive.")]
  [InlineData(100, 1, 150)]
  [InlineData(47, 2, 94)]
  [InlineData(133, 3, 332)]
  [InlineData(11, 4, 33)]
  [InlineData(5, 5, 17)]
  [InlineData(199, 6, 796)]
  public void Given_PositiveStages_When_ctor_Then_Augmented(int value, int stages, int expected)
  {
    BattleStatisticModel model = new(value, stages);
    Assert.Equal(value, model.Unmodified);
    Assert.Equal(stages, model.Stages);
    Assert.Equal(expected, model.Modified);
  }

  [Theory(DisplayName = "It should cap to the maximum stage when stages exceed the maximum limit.")]
  [InlineData(50, 7)]
  public void Given_PositiveExceeding_When_ctor_Then_MaximumCapped(int value, int stages)
  {
    BattleStatisticModel model = new(value, stages);
    Assert.Equal(value, model.Unmodified);
    Assert.Equal(stages, model.Stages);
    Assert.Equal(value * 4, model.Modified);
  }

  [Theory(DisplayName = "It should cap to the minimum stage when stages exceed the minimum limit.")]
  [InlineData(281, -9)]
  public void Given_NegativeExceeding_When_ctor_Then_MinimumCapped(int value, int stages)
  {
    BattleStatisticModel model = new(value, stages);
    Assert.Equal(value, model.Unmodified);
    Assert.Equal(stages, model.Stages);
    Assert.Equal(value / 4, model.Modified);
  }

  [Theory(DisplayName = "It should diminish the value when stages are negative.")]
  [InlineData(100, -1, 66)]
  [InlineData(47, -2, 23)]
  [InlineData(133, -3, 53)]
  [InlineData(11, -4, 3)]
  [InlineData(5, -5, 1)]
  [InlineData(199, -6, 49)]
  public void Given_NegativeStages_When_ctor_Then_Diminished(int value, int stages, int expected)
  {
    BattleStatisticModel model = new(value, stages);
    Assert.Equal(value, model.Unmodified);
    Assert.Equal(stages, model.Stages);
    Assert.Equal(expected, model.Modified);
  }

  [Theory(DisplayName = "It should not modify the value when there is no stage.")]
  [InlineData(100)]
  public void Given_NoStage_When_ctor_Then_Unmodified(int value)
  {
    BattleStatisticModel model = new(value, stages: 0);
    Assert.Equal(value, model.Unmodified);
    Assert.Equal(value, model.Modified);
    Assert.Equal(0, model.Stages);
  }
}
