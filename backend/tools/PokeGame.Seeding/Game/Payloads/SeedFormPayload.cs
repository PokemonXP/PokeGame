using CsvHelper.Configuration;
using PokeGame.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedFormPayload : CreateOrReplaceFormPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedFormPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(3).Default(string.Empty);
      Map(x => x.DisplayName).Index(4);
      Map(x => x.Description).Index(5);

      Map(x => x.Variety).Index(1).Default(string.Empty);
      Map(x => x.IsDefault).Index(2).Default(false);

      Map(x => x.IsBattleOnly).Index(6).Default(false);
      Map(x => x.IsMega).Index(7).Default(false);

      Map(x => x.Height).Index(8).Default(0);
      Map(x => x.Weight).Index(9).Default(0);

      References<TypesMap>(x => x.Types);
      References<AbilitiesMap>(x => x.Abilities);
      References<BaseStatisticsMap>(x => x.BaseStatistics);
      References<YieldMap>(x => x.Yield);
      References<SpritesMap>(x => x.Sprites);

      Map(x => x.Url).Index(32);
      Map(x => x.Notes).Index(33);
    }
  }

  private class AbilitiesMap : ClassMap<FormAbilitiesPayload>
  {
    public AbilitiesMap()
    {
      Map(x => x.Primary).Index(12).Default(string.Empty);
      Map(x => x.Secondary).Index(13);
      Map(x => x.Hidden).Index(14);
    }
  }

  private class BaseStatisticsMap : ClassMap<BaseStatisticsModel>
  {
    public BaseStatisticsMap()
    {
      Map(x => x.HP).Index(15).Default(0);
      Map(x => x.Attack).Index(16).Default(0);
      Map(x => x.Defense).Index(17).Default(0);
      Map(x => x.SpecialAttack).Index(18).Default(0);
      Map(x => x.SpecialDefense).Index(19).Default(0);
      Map(x => x.Speed).Index(20).Default(0);
    }
  }

  private class SpritesMap : ClassMap<SpritesModel>
  {
    public SpritesMap()
    {
      Map(x => x.Default).Index(28).Default(string.Empty);
      Map(x => x.Shiny).Index(29).Default(string.Empty);
      Map(x => x.Alternative).Index(30);
      Map(x => x.AlternativeShiny).Index(31);
    }
  }

  private class TypesMap : ClassMap<FormTypesModel>
  {
    public TypesMap()
    {
      Map(x => x.Primary).Index(10).Default(default(PokemonType));
      Map(x => x.Secondary).Index(11);
    }
  }

  private class YieldMap : ClassMap<YieldModel>
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
