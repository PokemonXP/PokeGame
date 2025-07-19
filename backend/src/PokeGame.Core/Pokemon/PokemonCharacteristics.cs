namespace PokeGame.Core.Pokemon;

public interface IPokemonCharacteristics
{
  PokemonCharacteristic Find(IIndividualValues individualValues, IPokemonSize size);
}

public class PokemonCharacteristics : IPokemonCharacteristics
{
  private static IPokemonCharacteristics? _instance = null;
  public static IPokemonCharacteristics Instance
  {
    get
    {
      _instance ??= new PokemonCharacteristics();
      return _instance;
    }
  }

  private readonly Dictionary<PokemonStatistic, string[]> _texts = new(capacity: 6)
  {
    [PokemonStatistic.HP] = ["Loves to eat", "Takes plenty of siestas", "Nods off a lot", "Scatters things often", "Likes to relax"],
    [PokemonStatistic.Attack] = ["Proud of its power", "Likes to thrash about", "A little quick tempered", "Likes to fight", "Quick tempered"],
    [PokemonStatistic.Defense] = ["Sturdy body", "Capable of taking hits", "Highly persistent", "Good endurance", "Good perseverance"],
    [PokemonStatistic.SpecialAttack] = ["Highly curious", "Mischievous", "Thoroughly cunning", "Often lost in thought", "Very finicky"],
    [PokemonStatistic.SpecialDefense] = ["Strong willed", "Somewhat vain", "Strongly defiant", "Hates to lose", "Somewhat stubborn"],
    [PokemonStatistic.Speed] = ["Likes to run", "Alert to sounds", "Impetuous and silly", "Somewhat of a clown", "Quick to flee"]
  };

  public PokemonCharacteristic Find(IIndividualValues individualValues, IPokemonSize size)
  {
    int maximum = new byte[]
    {
      individualValues.HP, individualValues.Attack, individualValues.Defense, individualValues.SpecialAttack, individualValues.SpecialDefense, individualValues.Speed
    }.Max();
    int index = maximum % 5;

    HashSet<PokemonStatistic> statistics = new(capacity: 6);
    if (individualValues.HP == maximum)
    {
      statistics.Add(PokemonStatistic.HP);
    }
    if (individualValues.Attack == maximum)
    {
      statistics.Add(PokemonStatistic.Attack);
    }
    if (individualValues.Defense == maximum)
    {
      statistics.Add(PokemonStatistic.Defense);
    }
    if (individualValues.SpecialAttack == maximum)
    {
      statistics.Add(PokemonStatistic.SpecialAttack);
    }
    if (individualValues.SpecialDefense == maximum)
    {
      statistics.Add(PokemonStatistic.SpecialDefense);
    }
    if (individualValues.Speed == maximum)
    {
      statistics.Add(PokemonStatistic.Speed);
    }

    PokemonStatistic statistic;
    if (statistics.Count == 1)
    {
      statistic = statistics.Single();
    }
    else
    {
      const int StatisticCount = 6;
      int value = (size.Height + size.Weight) % StatisticCount;
      bool found = false;
      int iterations = 0;
      do
      {
        statistic = (PokemonStatistic)value;
        if (statistics.Contains(statistic))
        {
          found = true;
        }
        else if (value == (StatisticCount - 1))
        {
          value = 0;
        }
        else
        {
          value++;
        }
        iterations++;
      } while (!found && iterations < StatisticCount);

      if (!found)
      {
        string individualValuesJoined = string.Join(',', individualValues.HP, individualValues.Attack, individualValues.Defense,
          individualValues.SpecialAttack, individualValues.SpecialDefense, individualValues.Speed);
        throw new InvalidOperationException($"The characteristic could not be determined for IVs '{individualValuesJoined}' and size 'Height={size.Height},Weight={size.Weight}'.");
      }
    }

    string text = _texts[statistic][index];
    return new PokemonCharacteristic(text);
  }
}

public record PokemonCharacteristic
{
  public const int MaximumLength = 32;

  public string Text { get; }

  public PokemonCharacteristic(string text)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(text, nameof(text));
    Text = text.Trim();
  }

  public override string ToString() => Text;
}
