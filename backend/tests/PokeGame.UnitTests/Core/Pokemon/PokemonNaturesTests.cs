using CsvHelper;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonNaturesTests : IAsyncLifetime
{
  private IReadOnlyDictionary<string, NatureInfo> _expected = new Dictionary<string, NatureInfo>();
  private readonly IPokemonNatures _natures = PokemonNatures.Instance;

  public PokemonNaturesTests()
  {
  }

  public async Task InitializeAsync()
  {
    using StreamReader reader = new("natures.csv", Encoding.UTF8);
    using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

    csv.Context.RegisterClassMap<NatureInfo.Map>();

    IAsyncEnumerable<NatureInfo> records = csv.GetRecordsAsync<NatureInfo>();

    Dictionary<string, NatureInfo> expected = [];
    await foreach (NatureInfo record in records)
    {
      expected[record.Name.Trim().ToLowerInvariant()] = record;
    }
    _expected = expected.AsReadOnly();
  }

  [Fact(DisplayName = "Find: it should return the nature found.")]
  public void Given_Exists_When_Find_Then_NatureReturned()
  {
    PokemonNature nature = _natures.Find("  LaX  ");
    Assert.Equal("Lax", nature.Name);
  }

  [Fact(DisplayName = "Find: it should throw ArgumentException when the nature is not found.")]
  public void Given_NotExists_When_Find_Then_ArgumentException()
  {
    var exception = Assert.Throws<ArgumentException>(() => _natures.Find("   EvIl "));
    Assert.StartsWith("The nature '   EvIl ' was not found.", exception.Message);
    Assert.Equal("name", exception.ParamName);
  }

  [Fact(DisplayName = "Get: it should return null when the nature does not exist.")]
  public void Given_NotExists_When_Get_Then_NullReturned()
  {
    Assert.Null(_natures.Get("Mischievous"));
  }

  [Fact(DisplayName = "Get: it should return the nature found.")]
  public void Given_Exists_When_Get_Then_NatureReturned()
  {
    PokemonNature? nature = _natures.Get(" dOcIlE   ");
    Assert.NotNull(nature);
    Assert.Equal("Docile", nature.Name);
  }

  [Fact(DisplayName = "It should be constructed with the correct natures.")]
  public void Given_Natures_When_ctor_Then_CorrectNatures()
  {
    foreach (KeyValuePair<string, NatureInfo> item in _expected)
    {
      PokemonNature nature = _natures.Find(item.Key);
      Assert.Equal(item.Value.Name, nature.Name);

      if (item.Value.IncreasedStatistic == item.Value.DecreasedStatistic)
      {
        Assert.Null(nature.IncreasedStatistic);
        Assert.Null(nature.DecreasedStatistic);
      }
      else
      {
        Assert.Equal(item.Value.IncreasedStatistic, nature.IncreasedStatistic);
        Assert.Equal(item.Value.DecreasedStatistic, nature.DecreasedStatistic);
      }

      if (item.Value.FavoriteFlavor == item.Value.DislikedFlavor)
      {
        Assert.Null(nature.FavoriteFlavor);
        Assert.Null(nature.DislikedFlavor);
      }
      else
      {
        Assert.Equal(item.Value.FavoriteFlavor, nature.FavoriteFlavor);
        Assert.Equal(item.Value.DislikedFlavor, nature.DislikedFlavor);
      }
    }
  }

  [Fact(DisplayName = "It should retrieve the correct nature by key.")]
  public void Given_Keys_When_get_Then_CorrectNatures()
  {
    PokemonNature[] natures =
    [
      _natures.Adamant, _natures.Bashful, _natures.Bold, _natures.Brave, _natures.Calm,
      _natures.Careful, _natures.Docile, _natures.Gentle, _natures.Hardy, _natures.Hasty,
      _natures.Impish, _natures.Jolly, _natures.Lax, _natures.Lonely, _natures.Mild,
      _natures.Modest, _natures.Naive, _natures.Naughty, _natures.Quiet, _natures.Quirky,
      _natures.Rash, _natures.Relaxed, _natures.Sassy, _natures.Serious, _natures.Timid
    ];
    foreach (PokemonNature nature in natures)
    {
      NatureInfo info = _expected[nature.Name.Trim().ToLowerInvariant()];

      if (info.IncreasedStatistic == info.DecreasedStatistic)
      {
        Assert.Null(nature.IncreasedStatistic);
        Assert.Null(nature.DecreasedStatistic);
      }
      else
      {
        Assert.Equal(info.IncreasedStatistic, nature.IncreasedStatistic);
        Assert.Equal(info.DecreasedStatistic, nature.DecreasedStatistic);
      }

      if (info.FavoriteFlavor == info.DislikedFlavor)
      {
        Assert.Null(nature.FavoriteFlavor);
        Assert.Null(nature.DislikedFlavor);
      }
      else
      {
        Assert.Equal(info.FavoriteFlavor, nature.FavoriteFlavor);
        Assert.Equal(info.DislikedFlavor, nature.DislikedFlavor);
      }
    }
  }

  [Fact(DisplayName = "ToList: it should return the correct nature list.")]
  public void Given_Natures_When_ToList_Then_NatureList()
  {
    int count = _natures.ToList().Select(nature => nature.Name).Distinct().Count();
    Assert.Equal(_expected.Count, count);
  }

  public Task DisposeAsync() => Task.CompletedTask;
}
