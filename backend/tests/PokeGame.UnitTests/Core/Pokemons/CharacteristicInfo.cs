using CsvHelper.Configuration;

namespace PokeGame.Core.Pokemons;

internal record CharacteristicInfo
{
  public int Modulo { get; set; }

  public string HP { get; set; } = string.Empty;
  public string Attack { get; set; } = string.Empty;
  public string Defense { get; set; } = string.Empty;
  public string SpecialAttack { get; set; } = string.Empty;
  public string SpecialDefense { get; set; } = string.Empty;
  public string Speed { get; set; } = string.Empty;

  public class Map : ClassMap<CharacteristicInfo>
  {
    public Map()
    {
      Map(x => x.Modulo).Index(0);

      Map(x => x.HP).Index(1);
      Map(x => x.Attack).Index(2);
      Map(x => x.Defense).Index(3);
      Map(x => x.SpecialAttack).Index(5);
      Map(x => x.SpecialDefense).Index(6);
      Map(x => x.Speed).Index(4);
    }
  }
}
