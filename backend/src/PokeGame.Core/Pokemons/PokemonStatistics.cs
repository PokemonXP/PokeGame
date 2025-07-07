namespace PokeGame.Core.Pokemons;

public record PokemonStatistics // TODO(fpion): unit tests
{
  private const double DecreaseMultiplier = 0.9;
  private const double IncreaseMultiplier = 1.1;

  public int HP { get; }
  public int Attack { get; }
  public int Defense { get; }
  public int SpecialAttack { get; }
  public int SpecialDefense { get; }
  public int Speed { get; }

  public PokemonStatistics(IBaseStatistics baseStatistics, IIndividualValues individualValues, IEffortValues effortValues, int level, PokemonNature nature)
  {
    HP = CalculateHP(baseStatistics.HP, individualValues.HP, effortValues.HP, level);
    Attack = CalculateOther(baseStatistics.Attack, individualValues.Attack, effortValues.Attack, level);
    Defense = CalculateOther(baseStatistics.Defense, individualValues.Defense, effortValues.Defense, level);
    SpecialAttack = CalculateOther(baseStatistics.SpecialAttack, individualValues.SpecialAttack, effortValues.SpecialAttack, level);
    SpecialDefense = CalculateOther(baseStatistics.SpecialDefense, individualValues.SpecialDefense, effortValues.SpecialDefense, level);
    Speed = CalculateOther(baseStatistics.Speed, individualValues.Speed, effortValues.Speed, level);

    PokemonStatistic increasedStatistic = nature.GetIncreasedStatistic();
    PokemonStatistic decreasedStatistic = nature.GetDecreasedStatistic();
    if (increasedStatistic != decreasedStatistic)
    {
      switch (increasedStatistic)
      {
        case PokemonStatistic.Attack:
          Attack = (int)Math.Floor(Attack * IncreaseMultiplier);
          break;
        case PokemonStatistic.Defense:
          Defense = (int)Math.Floor(Defense * IncreaseMultiplier);
          break;
        case PokemonStatistic.SpecialAttack:
          SpecialAttack = (int)Math.Floor(SpecialAttack * IncreaseMultiplier);
          break;
        case PokemonStatistic.SpecialDefense:
          SpecialDefense = (int)Math.Floor(SpecialDefense * IncreaseMultiplier);
          break;
        case PokemonStatistic.Speed:
          Speed = (int)Math.Floor(Speed * IncreaseMultiplier);
          break;
      }

      switch (decreasedStatistic)
      {
        case PokemonStatistic.Attack:
          Attack = (int)Math.Floor(Attack * DecreaseMultiplier);
          break;
        case PokemonStatistic.Defense:
          Defense = (int)Math.Floor(Defense * DecreaseMultiplier);
          break;
        case PokemonStatistic.SpecialAttack:
          SpecialAttack = (int)Math.Floor(SpecialAttack * DecreaseMultiplier);
          break;
        case PokemonStatistic.SpecialDefense:
          SpecialDefense = (int)Math.Floor(SpecialDefense * DecreaseMultiplier);
          break;
        case PokemonStatistic.Speed:
          Speed = (int)Math.Floor(Speed * DecreaseMultiplier);
          break;
      }
    }
  }

  public PokemonStatistics(Pokemon pokemon) : this(pokemon.BaseStatistics, pokemon.IndividualValues, pokemon.EffortValues, pokemon.Level, pokemon.Nature)
  {
  }

  private static int CalculateHP(int @base, byte individualValue, byte effortValue, int level)
  {
    return (int)Math.Floor(2.0 * @base + individualValue + Math.Floor(effortValue / 4.0) / 100.0) + level + 10;
  }
  private static int CalculateOther(int @base, byte individualValue, byte effortValue, int level) // TODO(fpion): refactor to include nature/multiplier
  {
    return (int)Math.Floor((2.0 * @base + individualValue + Math.Floor(effortValue / 4.0)) * level / 100.0) + 5;
  }
}
