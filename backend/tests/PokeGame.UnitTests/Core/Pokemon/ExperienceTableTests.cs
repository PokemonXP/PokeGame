using CsvHelper;
using PokeGame.Core.Species;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class ExperienceTableTests : IAsyncLifetime
{
  private LevelExperience[] _expected = [];
  private readonly IExperienceTable _table = ExperienceTable.Instance;

  public ExperienceTableTests()
  {
  }

  public async Task InitializeAsync()
  {
    using StreamReader reader = new("experience.csv", Encoding.UTF8);
    using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

    csv.Context.RegisterClassMap<LevelExperience.Map>();

    IAsyncEnumerable<LevelExperience> records = csv.GetRecordsAsync<LevelExperience>();

    List<LevelExperience> items = new(capacity: 100);
    await foreach (LevelExperience record in records)
    {
      items.Add(record);
    }
    _expected = items.ToArray();
  }

  [Theory(DisplayName = "GetLevel: it should return the correct level for the growth and current experience.")]
  [InlineData(GrowthRate.Erratic, 3, 1)]
  [InlineData(GrowthRate.Fast, 3, 1)]
  [InlineData(GrowthRate.Fluctuating, 3, 1)]
  [InlineData(GrowthRate.MediumFast, 3, 1)]
  [InlineData(GrowthRate.MediumSlow, 3, 1)]
  [InlineData(GrowthRate.Slow, 3, 1)]
  [InlineData(GrowthRate.Erratic, 100, 3)]
  [InlineData(GrowthRate.Fast, 100, 5)]
  [InlineData(GrowthRate.Fluctuating, 100, 5)]
  [InlineData(GrowthRate.MediumFast, 100, 4)]
  [InlineData(GrowthRate.MediumSlow, 100, 4)]
  [InlineData(GrowthRate.Slow, 100, 4)]
  public void Given_GrowthRateAndExperience_When_GetLevel_Then_CorrectLevel(GrowthRate growthRate, int experience, int level)
  {
    Assert.Equal(level, _table.GetLevel(growthRate, experience));
  }

  [Fact(DisplayName = "GetMaximumExperience: it should return the correct maximum experience for the growth rate and level.")]
  public void Given_GrowthRateAndLevel_When_GetMaximumExperience_Then_CorrectExperience()
  {
    foreach (GrowthRate growthRate in Enum.GetValues<GrowthRate>())
    {
      for (int level = 1; level <= 100; level++)
      {
        int expected = GetExpectedExperience(growthRate, level);
        int actual = _table.GetMaximumExperience(growthRate, level);
        if (expected != actual)
        {
          Assert.Fail($"The expected and actual maximum experience differ for growth rate '{growthRate}' at level {level}.");
        }
      }
    }
  }
  private int GetExpectedExperience(GrowthRate growthRate, int level) => growthRate switch
  {
    GrowthRate.Erratic => _expected[Math.Min(level, 99)].Erratic,
    GrowthRate.Fast => _expected[Math.Min(level, 99)].Fast,
    GrowthRate.MediumFast => _expected[Math.Min(level, 99)].MediumFast,
    GrowthRate.MediumSlow => _expected[Math.Min(level, 99)].MediumSlow,
    GrowthRate.Slow => _expected[Math.Min(level, 99)].Slow,
    GrowthRate.Fluctuating => _expected[Math.Min(level, 99)].Fluctuating,
    _ => throw new ArgumentException($"The growth rate '{growthRate}' is not valid.", nameof(growthRate)),
  };

  public Task DisposeAsync() => Task.CompletedTask;
}
