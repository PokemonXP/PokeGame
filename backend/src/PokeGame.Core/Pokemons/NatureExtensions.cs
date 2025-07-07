namespace PokeGame.Core.Pokemons;

public static class NatureExtensions // TODO(fpion): create a record/reference table by merging these extension methods with the enum
{
  public static PokemonStatistic GetDecreasedStatistic(this PokemonNature nature) => nature switch
  {
    PokemonNature.Hardy or PokemonNature.Bold or PokemonNature.Timid or PokemonNature.Modest or PokemonNature.Calm => PokemonStatistic.Attack,
    PokemonNature.Lonely or PokemonNature.Docile or PokemonNature.Hasty or PokemonNature.Mild or PokemonNature.Gentle => PokemonStatistic.Defense,
    PokemonNature.Adamant or PokemonNature.Impish or PokemonNature.Jolly or PokemonNature.Bashful or PokemonNature.Careful => PokemonStatistic.Speed,
    PokemonNature.Naughty or PokemonNature.Lax or PokemonNature.Naive or PokemonNature.Rash or PokemonNature.Quirky => PokemonStatistic.Attack,
    PokemonNature.Brave or PokemonNature.Relaxed or PokemonNature.Serious or PokemonNature.Quiet or PokemonNature.Sassy => PokemonStatistic.SpecialDefense,
    _ => throw new ArgumentException($"The Pokémon nature '{nature}' is not valid.", nameof(nature)),
  };
  public static PokemonStatistic GetIncreasedStatistic(this PokemonNature nature) => nature switch
  {
    PokemonNature.Hardy or PokemonNature.Lonely or PokemonNature.Brave or PokemonNature.Adamant or PokemonNature.Naughty => PokemonStatistic.Attack,
    PokemonNature.Bold or PokemonNature.Docile or PokemonNature.Relaxed or PokemonNature.Impish or PokemonNature.Lax => PokemonStatistic.Defense,
    PokemonNature.Timid or PokemonNature.Hasty or PokemonNature.Serious or PokemonNature.Jolly or PokemonNature.Naive => PokemonStatistic.Speed,
    PokemonNature.Modest or PokemonNature.Mild or PokemonNature.Quiet or PokemonNature.Bashful or PokemonNature.Rash => PokemonStatistic.Attack,
    PokemonNature.Calm or PokemonNature.Gentle or PokemonNature.Sassy or PokemonNature.Careful or PokemonNature.Quirky => PokemonStatistic.SpecialDefense,
    _ => throw new ArgumentException($"The Pokémon nature '{nature}' is not valid.", nameof(nature)),
  };
}
