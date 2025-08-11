using CsvHelper.Configuration;
using PokeGame.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Tools.Shared.Models;

public record SeedFormPayload : CreateOrReplaceFormPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedFormPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Name("id").Default(Guid.Empty);

      Map(x => x.Variety).Index(1).Name("variety").Default(string.Empty);
      Map(x => x.IsDefault).Index(2).Name("is_default").Default(false);

      Map(x => x.UniqueName).Index(3).Name("unique_name").Default(string.Empty);
      Map(x => x.DisplayName).Index(4).Name("display_name");
      Map(x => x.Description).Index(5).Name("description");

      Map(x => x.IsBattleOnly).Index(6).Name("is_battle_only").Default(false);
      Map(x => x.IsMega).Index(7).Name("is_mega").Default(false);

      Map(x => x.Height).Index(8).Name("height").Default(0);
      Map(x => x.Weight).Index(9).Name("weight").Default(0);

      References<TypesMap>(x => x.Types);
      References<AbilitiesMap>(x => x.Abilities);
      References<BaseStatisticsMap>(x => x.BaseStatistics);
      References<YieldMap>(x => x.Yield);
      References<SpritesMap>(x => x.Sprites);

      Map(x => x.Url).Name("url").Index(32);
      Map(x => x.Notes).Name("notes").Index(33);
    }
  }

  private class AbilitiesMap : ClassMap<FormAbilitiesPayload>
  {
    public AbilitiesMap()
    {
      Map(x => x.Primary).Index(12).Name("primary_ability").Default(string.Empty);
      Map(x => x.Secondary).Index(13).Name("secondary_ability");
      Map(x => x.Hidden).Index(14).Name("hidden_ability");
    }
  }

  private class BaseStatisticsMap : ClassMap<BaseStatisticsModel>
  {
    public BaseStatisticsMap()
    {
      Map(x => x.HP).Index(15).Name("base_hp").Default(0);
      Map(x => x.Attack).Index(16).Name("base_attack").Default(0);
      Map(x => x.Defense).Index(17).Name("base_defense").Default(0);
      Map(x => x.SpecialAttack).Name("base_special_attack").Index(18).Default(0);
      Map(x => x.SpecialDefense).Name("base_special_defense").Index(19).Default(0);
      Map(x => x.Speed).Index(20).Name("base_speed").Default(0);
    }
  }

  private class SpritesMap : ClassMap<SpritesModel>
  {
    public SpritesMap()
    {
      Map(x => x.Default).Index(28).Name("default_sprite").Default(string.Empty);
      Map(x => x.Shiny).Index(29).Name("default_shiny_sprite").Default(string.Empty);
      Map(x => x.Alternative).Index(30).Name("alternative_sprite");
      Map(x => x.AlternativeShiny).Name("alternative_shiny_sprite").Index(31);
    }
  }

  private class TypesMap : ClassMap<FormTypesModel>
  {
    public TypesMap()
    {
      Map(x => x.Primary).Index(10).Name("primary_type").Default(default(PokemonType));
      Map(x => x.Secondary).Index(11).Name("secondary_type");
    }
  }

  private class YieldMap : ClassMap<YieldModel>
  {
    public YieldMap()
    {
      Map(x => x.Experience).Index(21).Name("experience_yield");

      Map(x => x.HP).Index(22).Name("hp_yield").Default(0);
      Map(x => x.Attack).Index(23).Name("attack_yield").Default(0);
      Map(x => x.Defense).Index(24).Name("defense_yield").Default(0);
      Map(x => x.SpecialAttack).Name("special_attack_yield").Index(25).Default(0);
      Map(x => x.SpecialDefense).Name("special_defense_yield").Index(26).Default(0);
      Map(x => x.Speed).Index(27).Name("speed_yield").Default(0);
    }
  }
}
