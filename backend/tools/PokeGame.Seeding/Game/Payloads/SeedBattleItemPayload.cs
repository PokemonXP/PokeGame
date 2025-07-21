using CsvHelper.Configuration;
using PokeGame.Core.Items.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedBattleItemPayload : CreateOrReplaceItemPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedBattleItemPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);

      References<BattleItemPropertiesMap>(x => x.BattleItem);

      Map(x => x.Sprite).Index(14);
      Map(x => x.Url).Index(15);
      Map(x => x.Notes).Index(16);
    }
  }

  private class BattleItemPropertiesMap : ClassMap<BattleItemPropertiesModel>
  {
    public BattleItemPropertiesMap()
    {
      Map(x => x.Attack).Index(5).Default(0);
      Map(x => x.Defense).Index(6).Default(0);
      Map(x => x.SpecialAttack).Index(7).Default(0);
      Map(x => x.SpecialDefense).Index(8).Default(0);
      Map(x => x.Speed).Index(9).Default(0);
      Map(x => x.Accuracy).Index(10).Default(0);
      Map(x => x.Evasion).Index(11).Default(0);
      Map(x => x.Critical).Index(12).Default(0);
      Map(x => x.GuardTurns).Index(13).Default(0);
    }
  }
}
