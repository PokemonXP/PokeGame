using CsvHelper.Configuration;

namespace PokeGame.Seeding.Game.Payloads;

internal class FormPayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Variety { get; set; } = string.Empty;
  public bool IsDefault { get; set; }

  public bool IsBattleOnly { get; set; }
  public bool IsMega { get; set; }

  public int Height { get; set; }
  public int Weight { get; set; }

  public TypesPayload Types { get; set; } = new();
  public AbilitiesPayload Abilities { get; set; } = new();
  public BaseStatisticsPayload BaseStatistics { get; set; } = new();
  public YieldPayload Yield { get; set; } = new();
  public SpritesPayload Sprites { get; set; } = new();

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is FormPayload form && form.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<FormPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0);

      Map(x => x.UniqueName).Index(3);
      Map(x => x.DisplayName).Index(4);
      Map(x => x.Description).Index(5);

      Map(x => x.Variety).Index(1);
      Map(x => x.IsDefault).Index(2);

      Map(x => x.IsBattleOnly).Index(6);
      Map(x => x.IsMega).Index(7);

      Map(x => x.Height).Index(8);
      Map(x => x.Weight).Index(9);

      References<TypesMap>(x => x.Types);
      References<AbilitiesMap>(x => x.Abilities);
      References<BaseStatisticsMap>(x => x.BaseStatistics);
      References<YieldMap>(x => x.Yield);
      References<SpritesMap>(x => x.Sprites);

      Map(x => x.Url).Index(32);
      Map(x => x.Notes).Index(33);
    }
  }

  private class AbilitiesMap : ClassMap<AbilitiesPayload>
  {
    public AbilitiesMap()
    {
      Map(x => x.Primary).Index(12);
      Map(x => x.Secondary).Index(13);
      Map(x => x.Hidden).Index(14);
    }
  }

  private class BaseStatisticsMap : ClassMap<BaseStatisticsPayload>
  {
    public BaseStatisticsMap()
    {
      Map(x => x.HP).Index(15);
      Map(x => x.Attack).Index(16);
      Map(x => x.Defense).Index(17);
      Map(x => x.SpecialAttack).Index(18);
      Map(x => x.SpecialDefense).Index(19);
      Map(x => x.Speed).Index(20);
    }
  }

  private class SpritesMap : ClassMap<SpritesPayload>
  {
    public SpritesMap()
    {
      Map(x => x.Default).Index(28);
      Map(x => x.DefaultShiny).Index(29);
      Map(x => x.Alternative).Index(30);
      Map(x => x.AlternativeShiny).Index(31);
    }
  }

  private class TypesMap : ClassMap<TypesPayload>
  {
    public TypesMap()
    {
      Map(x => x.Primary).Index(10);
      Map(x => x.Secondary).Index(11);
    }
  }

  private class YieldMap : ClassMap<YieldPayload>
  {
    public YieldMap()
    {
      Map(x => x.Experience).Index(21);

      Map(x => x.HP).Index(22).Default(0);
      Map(x => x.Attack).Index(23).Default(0);
      Map(x => x.Defense).Index(24).Default(0);
      Map(x => x.SpecialAttack).Index(25).Default(0);
      Map(x => x.SpecialDefense).Index(26).Default(0);
      Map(x => x.Speed).Index(27).Default(0);
    }
  }
}
