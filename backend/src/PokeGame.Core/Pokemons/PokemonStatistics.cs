namespace PokeGame.Core.Pokemons;

public record PokemonStatistics // TODO(fpion): unit tests
{
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
  }

  public PokemonStatistics(Pokemon pokemon) : this(pokemon.BaseStatistics, pokemon.IndividualValues, pokemon.EffortValues, pokemon.Level, pokemon.Nature)
  {
  }

  private static int CalculateHP(int @base, byte individualValue, byte effortValue, int level)
  {
    return (int)Math.Floor(2.0 * @base + individualValue + Math.Floor(effortValue / 4.0) / 100.0) + level + 10;
  }
  private static int CalculateOther(int @base, byte individualValue, byte effortValue, int level)
  {
    return (int)Math.Floor((2.0 * @base + individualValue + Math.Floor(effortValue / 4.0)) * level / 100.0) + 5; // TODO(fpion): nature
  }
}
