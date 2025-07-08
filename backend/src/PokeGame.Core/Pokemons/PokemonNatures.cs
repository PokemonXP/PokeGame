namespace PokeGame.Core.Pokemons;

public interface IPokemonNatures
{
  PokemonNature Adamant => Find(nameof(Adamant));
  PokemonNature Bashful => Find(nameof(Adamant));
  PokemonNature Bold => Find(nameof(Adamant));
  PokemonNature Brave => Find(nameof(Adamant));
  PokemonNature Calm => Find(nameof(Adamant));
  PokemonNature Careful => Find(nameof(Adamant));
  PokemonNature Docile => Find(nameof(Adamant));
  PokemonNature Gentle => Find(nameof(Adamant));
  PokemonNature Hardy => Find(nameof(Adamant));
  PokemonNature Hasty => Find(nameof(Adamant));
  PokemonNature Impish => Find(nameof(Adamant));
  PokemonNature Jolly => Find(nameof(Adamant));
  PokemonNature Lax => Find(nameof(Adamant));
  PokemonNature Lonely => Find(nameof(Adamant));
  PokemonNature Mild => Find(nameof(Adamant));
  PokemonNature Modest => Find(nameof(Adamant));
  PokemonNature Naive => Find(nameof(Adamant));
  PokemonNature Naughty => Find(nameof(Adamant));
  PokemonNature Quiet => Find(nameof(Adamant));
  PokemonNature Quirky => Find(nameof(Adamant));
  PokemonNature Rash => Find(nameof(Adamant));
  PokemonNature Relaxed => Find(nameof(Adamant));
  PokemonNature Sassy => Find(nameof(Adamant));
  PokemonNature Serious => Find(nameof(Adamant));
  PokemonNature Timid => Find(nameof(Adamant));

  PokemonNature Find(string name);
  PokemonNature? Get(string name);
  IReadOnlyCollection<PokemonNature> ToList();
}

public class PokemonNatures : IPokemonNatures
{
  private static IPokemonNatures? _instance = null;
  public static IPokemonNatures Instance
  {
    get
    {
      _instance ??= new PokemonNatures();
      return _instance;
    }
  }

  private readonly Dictionary<string, PokemonNature> _natures = new(capacity: 25);

  public PokemonNature Adamant => Find(nameof(Adamant));
  public PokemonNature Bashful => Find(nameof(Bashful));
  public PokemonNature Bold => Find(nameof(Bold));
  public PokemonNature Brave => Find(nameof(Brave));
  public PokemonNature Calm => Find(nameof(Calm));
  public PokemonNature Careful => Find(nameof(Careful));
  public PokemonNature Docile => Find(nameof(Docile));
  public PokemonNature Gentle => Find(nameof(Gentle));
  public PokemonNature Hardy => Find(nameof(Hardy));
  public PokemonNature Hasty => Find(nameof(Hasty));
  public PokemonNature Impish => Find(nameof(Impish));
  public PokemonNature Jolly => Find(nameof(Jolly));
  public PokemonNature Lax => Find(nameof(Lax));
  public PokemonNature Lonely => Find(nameof(Lonely));
  public PokemonNature Mild => Find(nameof(Mild));
  public PokemonNature Modest => Find(nameof(Modest));
  public PokemonNature Naive => Find(nameof(Naive));
  public PokemonNature Naughty => Find(nameof(Naughty));
  public PokemonNature Quiet => Find(nameof(Quiet));
  public PokemonNature Quirky => Find(nameof(Quirky));
  public PokemonNature Rash => Find(nameof(Rash));
  public PokemonNature Relaxed => Find(nameof(Relaxed));
  public PokemonNature Sassy => Find(nameof(Sassy));
  public PokemonNature Serious => Find(nameof(Serious));
  public PokemonNature Timid => Find(nameof(Timid));

  private PokemonNatures()
  {
    _natures["adamant"] = new PokemonNature("Adamant", PokemonStatistic.Attack, PokemonStatistic.SpecialAttack, Flavor.Spicy, Flavor.Dry);
    _natures["bashful"] = new PokemonNature("Bashful", PokemonStatistic.SpecialAttack);
    _natures["bold"] = new PokemonNature("Bold", PokemonStatistic.Defense, PokemonStatistic.Attack, Flavor.Sour, Flavor.Spicy);
    _natures["brave"] = new PokemonNature("Brave", PokemonStatistic.Attack, PokemonStatistic.Speed, Flavor.Spicy, Flavor.Sweet);
    _natures["calm"] = new PokemonNature("Calm", PokemonStatistic.SpecialDefense, PokemonStatistic.Attack, Flavor.Bitter, Flavor.Spicy);
    _natures["careful"] = new PokemonNature("Careful", PokemonStatistic.SpecialDefense, PokemonStatistic.SpecialAttack, Flavor.Bitter, Flavor.Dry);
    _natures["docile"] = new PokemonNature("Docile");
    _natures["gentle"] = new PokemonNature("Gentle", PokemonStatistic.SpecialDefense, PokemonStatistic.Defense, Flavor.Bitter, Flavor.Sour);
    _natures["hardy"] = new PokemonNature("Hardy");
    _natures["hasty"] = new PokemonNature("Hasty", PokemonStatistic.Speed, PokemonStatistic.Defense, Flavor.Sweet, Flavor.Sour);
    _natures["impish"] = new PokemonNature("Impish", PokemonStatistic.Defense, PokemonStatistic.SpecialAttack, Flavor.Sour, Flavor.Dry);
    _natures["jolly"] = new PokemonNature("Jolly", PokemonStatistic.Speed, PokemonStatistic.SpecialAttack, Flavor.Sweet, Flavor.Dry);
    _natures["lax"] = new PokemonNature("Lax", PokemonStatistic.Defense, PokemonStatistic.SpecialDefense, Flavor.Sour, Flavor.Bitter);
    _natures["lonely"] = new PokemonNature("Lonely", PokemonStatistic.Attack, PokemonStatistic.Defense, Flavor.Spicy, Flavor.Sour);
    _natures["mild"] = new PokemonNature("Mild", PokemonStatistic.SpecialAttack, PokemonStatistic.Defense, Flavor.Dry, Flavor.Sour);
    _natures["modest"] = new PokemonNature("Modest", PokemonStatistic.SpecialAttack, PokemonStatistic.Attack, Flavor.Dry, Flavor.Spicy);
    _natures["naive"] = new PokemonNature("Naive", PokemonStatistic.Speed, PokemonStatistic.SpecialDefense, Flavor.Sweet, Flavor.Bitter);
    _natures["naughty"] = new PokemonNature("Naughty", PokemonStatistic.Attack, PokemonStatistic.SpecialDefense, Flavor.Spicy, Flavor.Bitter);
    _natures["quiet"] = new PokemonNature("Quiet", PokemonStatistic.SpecialAttack, PokemonStatistic.Speed, Flavor.Dry, Flavor.Sweet);
    _natures["quirky"] = new PokemonNature("Quirky", PokemonStatistic.SpecialDefense);
    _natures["rash"] = new PokemonNature("Rash", PokemonStatistic.SpecialAttack, PokemonStatistic.SpecialDefense, Flavor.Dry, Flavor.Bitter);
    _natures["relaxed"] = new PokemonNature("Relaxed", PokemonStatistic.Defense, PokemonStatistic.Speed, Flavor.Sour, Flavor.Sweet);
    _natures["sassy"] = new PokemonNature("Sassy", PokemonStatistic.SpecialDefense, PokemonStatistic.Speed, Flavor.Bitter, Flavor.Sweet);
    _natures["serious"] = new PokemonNature("Serious");
    _natures["timid"] = new PokemonNature("Timid", PokemonStatistic.Speed, PokemonStatistic.Attack, Flavor.Sweet, Flavor.Spicy);
  }

  public PokemonNature Find(string name) => Get(name) ?? throw new ArgumentException($"The nature '{name}' could not be found.", nameof(name));
  public PokemonNature? Get(string name) => _natures.TryGetValue(name.Trim().ToLowerInvariant(), out PokemonNature? nature) ? nature : null;
  public IReadOnlyCollection<PokemonNature> ToList() => _natures.Values;
}

public record PokemonNature
{
  private const double DecreaseMultiplier = 0.9;
  private const double IncreaseMultiplier = 1.1;

  public string Name { get; }
  public PokemonStatistic? IncreasedStatistic { get; }
  public PokemonStatistic? DecreasedStatistic { get; }
  public Flavor? FavoriteFlavor { get; }
  public Flavor? DislikedFlavor { get; }

  public PokemonNature(
    string name,
    PokemonStatistic? increasedStatistic = null,
    PokemonStatistic? decreasedStatistic = null,
    Flavor? favoriteFlavor = null,
    Flavor? dislikedFlavor = null)
  {
    Name = name;
    IncreasedStatistic = increasedStatistic;
    DecreasedStatistic = decreasedStatistic;
    FavoriteFlavor = favoriteFlavor;
    DislikedFlavor = dislikedFlavor;
  }

  public double GetMultiplier(PokemonStatistic statistic)
  {
    if (statistic == IncreasedStatistic)
    {
      return IncreaseMultiplier;
    }
    else if (statistic == DecreasedStatistic)
    {
      return DecreaseMultiplier;
    }
    return 1.0;
  }

  public override string ToString() => Name;
}
