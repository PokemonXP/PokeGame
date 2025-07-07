using PokeGame.Core.Species;

namespace PokeGame.Core.Pokemons;

public interface IExperienceTable
{
  int GetLevel(GrowthRate growthRate, int experience);
  int GetMaximumExperience(GrowthRate growthRate, int level);
}

public class ExperienceTable : IExperienceTable
{
  private const int MaximumLevel = 100;

  private static IExperienceTable? _instance = null;
  public static IExperienceTable Instance
  {
    get
    {
      _instance ??= new ExperienceTable();
      return _instance;
    }
  }

  private readonly Dictionary<GrowthRate, int[]> _thresholds = [];

  private ExperienceTable()
  {
    int[] erratic = new int[MaximumLevel];
    int[] fast = new int[MaximumLevel];
    int[] fluctuating = new int[MaximumLevel];
    int[] mediumFast = new int[MaximumLevel];
    int[] mediumSlow = new int[MaximumLevel];
    int[] slow = new int[MaximumLevel];
    for (int level = 2; level <= MaximumLevel; level++)
    {
      erratic[level - 1] = CalculateErratic(level);
      fast[level - 1] = CalculateFast(level);
      fluctuating[level - 1] = CalculateFluctuating(level);
      mediumFast[level - 1] = CalculateMediumFast(level);
      mediumSlow[level - 1] = CalculateMediumSlow(level);
      slow[level - 1] = CalculateSlow(level);
    }
    _thresholds[GrowthRate.Erratic] = erratic.ToArray();
    _thresholds[GrowthRate.Fast] = fast.ToArray();
    _thresholds[GrowthRate.Fluctuating] = fluctuating.ToArray();
    _thresholds[GrowthRate.MediumFast] = mediumFast.ToArray();
    _thresholds[GrowthRate.MediumSlow] = mediumSlow.ToArray();
    _thresholds[GrowthRate.Slow] = slow.ToArray();
  }
  private static int CalculateErratic(int level)
  {
    double experience;
    double cube = Math.Pow(level, 3);
    if (level < 50)
    {
      experience = cube * (100.0 - level) / 50.0;
    }
    else if (level < 68)
    {
      experience = cube * (150.0 - level) / 100.0;
    }
    else if (level < 98)
    {
      experience = cube * Math.Floor((1911 - 10.0 * level) / 3.0) / 500.0;
    }
    else
    {
      experience = cube * (160.0 - level) / 100.0;
    }
    return (int)Math.Floor(experience);
  }
  private static int CalculateFast(int level) => (int)Math.Floor(4.0 * Math.Pow(level, 3) / 5.0);
  private static int CalculateFluctuating(int level)
  {
    double experience;
    double cube = Math.Pow(level, 3);
    if (level < 15)
    {
      experience = (cube * Math.Floor((level + 1) / 3.0) + 24.0) / 50.0;
    }
    else if (level < 36)
    {
      experience = cube * (level + 14.0) / 50.0;
    }
    else
    {
      experience = cube * (Math.Floor(level / 2.0) + 32.0) / 50.0;
    }
    return (int)Math.Floor(experience);
  }
  private static int CalculateMediumFast(int level) => (int)Math.Floor(Math.Pow(level, 3));
  private static int CalculateMediumSlow(int level)
  {
    double experience = 0;
    experience += 6.0 * Math.Pow(level, 3) / 5.0;
    experience -= 15.0 * Math.Pow(level, 2);
    experience += 100.0 * level;
    experience -= 140.0;
    return (int)Math.Floor(experience);
  }
  private static int CalculateSlow(int level) => (int)Math.Floor(5.0 * Math.Pow(level, 3) / 4.0);

  public int GetLevel(GrowthRate growthRate, int experience)
  {
    if (!_thresholds.TryGetValue(growthRate, out int[]? thresholds))
    {
      throw new ArgumentException($"The growth rate '{growthRate}' is not valid.", nameof(growthRate));
    }
    ArgumentOutOfRangeException.ThrowIfNegative(experience);

    for (int level = 0; level < MaximumLevel; level++)
    {
      if (experience < thresholds[level])
      {
        return level;
      }
    }

    return MaximumLevel;
  }
  public int GetMaximumExperience(GrowthRate growthRate, int level)
  {
    if (!_thresholds.TryGetValue(growthRate, out int[]? thresholds))
    {
      throw new ArgumentException($"The growth rate '{growthRate}' is not valid.", nameof(growthRate));
    }
    if (level < 1 || level > 100)
    {
      throw new ArgumentOutOfRangeException(nameof(level));
    }

    return thresholds[Math.Min(level + 1, 100)];
  }
}
