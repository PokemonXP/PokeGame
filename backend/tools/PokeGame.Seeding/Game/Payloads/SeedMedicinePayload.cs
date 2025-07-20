using CsvHelper.Configuration;
using PokeGame.Core.Items.Models;

namespace PokeGame.Seeding.Game.Payloads;

internal record SeedMedicinePayload : CreateOrReplaceItemPayload
{
  public Guid Id { get; set; }

  public class Map : ClassMap<SeedMedicinePayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);

      References<MedicinePropertiesMap>(x => x.Medicine);

      Map(x => x.Sprite).Index(14);

      Map(x => x.Url).Index(15);
      Map(x => x.Notes).Index(16);
    }

    private class MedicinePropertiesMap : ClassMap<MedicinePropertiesModel>
    {
      public MedicinePropertiesMap()
      {
        Map(x => x.IsHerbal).Index(5).Default(false);

        Map(x => x.Healing).Index(6).Default(0);
        Map(x => x.IsHealingPercentage).Index(7).Default(false);
        Map(x => x.Revives).Index(8).Default(false);

        Map(x => x.StatusCondition).Index(9);
        Map(x => x.AllConditions).Index(10).Default(false);

        Map(x => x.PowerPoints).Index(11).Default(0);
        Map(x => x.IsPowerPointPercentage).Index(12).Default(false);
        Map(x => x.RestoreAllMoves).Index(13).Default(false);
      }
    }
  }
}
