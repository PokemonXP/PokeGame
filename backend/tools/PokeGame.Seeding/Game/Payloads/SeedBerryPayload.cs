using CsvHelper.Configuration;
using PokeGame.Core.Items.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedBerryPayload : CreateOrReplaceItemPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedBerryPayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);

      References<BerryPropertiesMap>(x => x.Berry);

      Map(x => x.Sprite).Index(21);
      Map(x => x.Url).Index(22);
      Map(x => x.Notes).Index(23);
    }
  }

  private class BerryPropertiesMap : ClassMap<BerryPropertiesModel>
  {
    public BerryPropertiesMap()
    {
      Map(x => x.Healing).Index(5).Default(0);
      Map(x => x.IsHealingPercentage).Index(6).Default(false);

      Map(x => x.StatusCondition).Index(7);
      Map(x => x.AllConditions).Index(8).Default(false);
      Map(x => x.CureConfusion).Index(9).Default(false);

      Map(x => x.PowerPoints).Index(10).Default(0);

      Map(x => x.Attack).Index(11).Default(0);
      Map(x => x.Defense).Index(12).Default(0);
      Map(x => x.SpecialAttack).Index(13).Default(0);
      Map(x => x.SpecialDefense).Index(14).Default(0);
      Map(x => x.Speed).Index(15).Default(0);
      Map(x => x.Accuracy).Index(16).Default(0);
      Map(x => x.Evasion).Index(17).Default(0);
      Map(x => x.Critical).Index(18).Default(0);

      Map(x => x.LowerEffortValues).Index(19);
      Map(x => x.RaiseFriendship).Index(20).Default(false);
    }
  }
}
