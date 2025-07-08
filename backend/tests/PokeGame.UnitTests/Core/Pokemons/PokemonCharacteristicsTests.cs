using CsvHelper;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonCharacteristicsTests : IAsyncLifetime
{
  private readonly CharacteristicInfo[] _characteristics = new CharacteristicInfo[5];
  private readonly IPokemonCharacteristics _instance = PokemonCharacteristics.Instance;

  public async Task InitializeAsync()
  {
    using StreamReader reader = new("characteristics.csv", Encoding.UTF8);
    using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

    csv.Context.RegisterClassMap<CharacteristicInfo.Map>();

    IAsyncEnumerable<CharacteristicInfo> records = csv.GetRecordsAsync<CharacteristicInfo>();
    await foreach (CharacteristicInfo record in records)
    {
      _characteristics[record.Modulo] = record;
    }
  }

  [Fact(DisplayName = "It should construct the correct text table.")]
  public void Given_Characteristics_When_ctor_Then_CorrectTexts()
  {
    IndividualValues individualValues;
    PokemonSize size = new();
    for (byte iv = 1; iv <= 31; iv++)
    {
      int modulo = iv % 5;

      individualValues = new(iv, 0, 0, 0, 0, 0);
      Assert.Equal(_characteristics[modulo].HP, _instance.Find(individualValues, size).Text);

      individualValues = new(0, iv, 0, 0, 0, 0);
      Assert.Equal(_characteristics[modulo].Attack, _instance.Find(individualValues, size).Text);

      individualValues = new(0, 0, iv, 0, 0, 0);
      Assert.Equal(_characteristics[modulo].Defense, _instance.Find(individualValues, size).Text);

      individualValues = new(0, 0, 0, iv, 0, 0);
      Assert.Equal(_characteristics[modulo].SpecialAttack, _instance.Find(individualValues, size).Text);

      individualValues = new(0, 0, 0, 0, iv, 0);
      Assert.Equal(_characteristics[modulo].SpecialDefense, _instance.Find(individualValues, size).Text);

      individualValues = new(0, 0, 0, 0, 0, iv);
      Assert.Equal(_characteristics[modulo].Speed, _instance.Find(individualValues, size).Text);
    }

    individualValues = new(0, 0, 0, 0, 0, 0);
    Assert.Equal(_characteristics[0].HP, _instance.Find(individualValues, new PokemonSize(0, 0)).Text);
    Assert.Equal(_characteristics[0].Attack, _instance.Find(individualValues, new PokemonSize(1, 0)).Text);
    Assert.Equal(_characteristics[0].Defense, _instance.Find(individualValues, new PokemonSize(2, 0)).Text);
    Assert.Equal(_characteristics[0].SpecialAttack, _instance.Find(individualValues, new PokemonSize(3, 0)).Text);
    Assert.Equal(_characteristics[0].SpecialDefense, _instance.Find(individualValues, new PokemonSize(4, 0)).Text);
    Assert.Equal(_characteristics[0].Speed, _instance.Find(individualValues, new PokemonSize(5, 0)).Text);
  }

  [Theory(DisplayName = "It should return the correct text when many IVs are max.")]
  [InlineData(27, 27, 25, 21, 25, 27, 103, 66, "A little quick tempered")]
  [InlineData(27, 27, 25, 21, 25, 27, 40, 200, "Nods off a lot")]
  [InlineData(27, 27, 25, 21, 25, 27, 22, 7, "Impetuous and silly")]
  public void Given_ManyMaxStatistics_When_Find_Then_CorrectText(
    int hp, int attack, int defense, int specialAttack, int specialDefense, int speed, int height, int weight, string expected)
  {
    IndividualValues individualValues = new((byte)hp, (byte)attack, (byte)defense, (byte)specialAttack, (byte)specialDefense, (byte)speed);
    PokemonSize size = new((byte)height, (byte)weight);
    Assert.Equal(expected, _instance.Find(individualValues, size).Text);
  }

  [Theory(DisplayName = "It should return the correct text when only one IV is max.")]
  [InlineData(26, 28, 25, 22, 25, 26, "Likes to fight")]
  public void Given_SingleMaxStatistic_When_Find_Then_CorrectText(int hp, int attack, int defense, int specialAttack, int specialDefense, int speed, string expected)
  {
    IndividualValues individualValues = new((byte)hp, (byte)attack, (byte)defense, (byte)specialAttack, (byte)specialDefense, (byte)speed);
    PokemonSize size = new();
    Assert.Equal(expected, _instance.Find(individualValues, size).Text);
  }

  public Task DisposeAsync() => Task.CompletedTask;
}
