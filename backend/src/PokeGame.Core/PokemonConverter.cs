using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;

namespace PokeGame.Core;

public interface IPokemonConverter
{
  string FromCategory(PokemonCategory category);
  PokemonCategory ToCategory(string value);

  string FromGrowthRate(GrowthRate growthRate);
  GrowthRate ToGrowthRate(string value);

  string FromItemCategory(ItemCategory category);
  ItemCategory ToItemCategory(string value);

  string FromMoveCategory(MoveCategory category);
  MoveCategory ToMoveCategory(string value);

  string FromStatistic(PokemonStatistic statistic);
  PokemonStatistic ToStatistic(string value);

  string FromStatusCondition(StatusCondition condition);
  StatusCondition ToStatusCondition(string value);

  string FromTrainerGender(TrainerGender gender);
  TrainerGender ToTrainerGender(string value);

  string FromType(PokemonType type);
  PokemonType ToType(string value);

  string FromVolatileCondition(VolatileCondition condition);
  VolatileCondition ToVolatileCondition(string value);
}

public class PokemonConverter : IPokemonConverter
{
  private static IPokemonConverter? _instance = null;
  public static IPokemonConverter Instance
  {
    get
    {
      _instance ??= new PokemonConverter();
      return _instance;
    }
  }

  public string FromCategory(PokemonCategory category) => category.ToString().ToLowerInvariant();
  public PokemonCategory ToCategory(string value) => Enum.Parse<PokemonCategory>(value.Capitalize());

  public string FromGrowthRate(GrowthRate growthRate) => growthRate switch
  {
    GrowthRate.Erratic => "slow-then-very-fast",
    GrowthRate.Fluctuating => "fast-then-very-slow",
    GrowthRate.MediumFast => "medium",
    GrowthRate.MediumSlow => "medium-slow",
    _ => growthRate.ToString().ToLowerInvariant(),
  };
  public GrowthRate ToGrowthRate(string value) => value switch
  {
    "fast-then-very-slow" => GrowthRate.Fluctuating,
    "medium" => GrowthRate.MediumFast,
    "medium-slow" => GrowthRate.MediumSlow,
    "slow-then-very-fast" => GrowthRate.Erratic,
    _ => Enum.Parse<GrowthRate>(value.Capitalize()),
  };

  public string FromItemCategory(ItemCategory category) => category switch
  {
    ItemCategory.BattleItem => "battle",
    ItemCategory.KeyItem => "key",
    ItemCategory.Material => "material",
    ItemCategory.OtherItem => "other",
    ItemCategory.PicnicItem => "picnic",
    ItemCategory.PokeBall => "poke-ball",
    ItemCategory.TechnicalMachine => "tm",
    _ => category.ToString().ToLowerInvariant(),
  };
  public ItemCategory ToItemCategory(string value) => value switch
  {
    "battle" => ItemCategory.BattleItem,
    "key" => ItemCategory.KeyItem,
    "material" => ItemCategory.Material,
    "other" => ItemCategory.OtherItem,
    "picnic" => ItemCategory.PicnicItem,
    "poke-ball" => ItemCategory.PokeBall,
    "tm" => ItemCategory.TechnicalMachine,
    _ => Enum.Parse<ItemCategory>(value.Capitalize()),
  };

  public string FromMoveCategory(MoveCategory category) => category.ToString().ToLowerInvariant();
  public MoveCategory ToMoveCategory(string value) => Enum.Parse<MoveCategory>(value.Capitalize());

  public string FromStatistic(PokemonStatistic statistic) => statistic switch
  {
    PokemonStatistic.HP => "hp",
    PokemonStatistic.SpecialAttack => "special-attack",
    PokemonStatistic.SpecialDefense => "special-defense",
    _ => statistic.ToString().ToLowerInvariant(),
  };
  public PokemonStatistic ToStatistic(string value) => value switch
  {
    "hp" => PokemonStatistic.HP,
    "special-attack" => PokemonStatistic.SpecialAttack,
    "special-defense" => PokemonStatistic.SpecialDefense,
    _ => Enum.Parse<PokemonStatistic>(value.Capitalize()),
  };

  public string FromStatusCondition(StatusCondition condition) => condition.ToString().ToLowerInvariant();
  public StatusCondition ToStatusCondition(string value) => Enum.Parse<StatusCondition>(value.Capitalize());

  public string FromTrainerGender(TrainerGender gender) => gender.ToString().ToLowerInvariant();
  public TrainerGender ToTrainerGender(string value) => Enum.Parse<TrainerGender>(value.Capitalize());

  public string FromType(PokemonType type) => type.ToString().ToLowerInvariant();
  public PokemonType ToType(string value) => Enum.Parse<PokemonType>(value.Capitalize());

  public string FromVolatileCondition(VolatileCondition condition) => condition switch
  {
    VolatileCondition.BadlyPoisoned => "badly-poisoned",
    _ => condition.ToString().ToLowerInvariant(),
  };
  public VolatileCondition ToVolatileCondition(string value) => value switch
  {
    "badly-poisoned" => VolatileCondition.BadlyPoisoned,
    _ => Enum.Parse<VolatileCondition>(value.Capitalize()),
  };
}
