using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Items.Models;

public class ItemModel : AggregateModel
{
  public ItemCategory Category { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int Price { get; set; }

  public BattleItemPropertiesModel? BattleItem { get; set; }
  public BerryPropertiesModel? Berry { get; set; }
  public KeyItemPropertiesModel? KeyItem { get; set; }
  public MaterialPropertiesModel? Material { get; set; }
  public MedicinePropertiesModel? Medicine { get; set; }
  public OtherItemPropertiesModel? OtherItem { get; set; }
  public PokeBallPropertiesModel? PokeBall { get; set; }
  public TechnicalMachinePropertiesModel? TechnicalMachine { get; set; }
  public TreasurePropertiesModel? Treasure { get; set; }

  public string? Sprite { get; set; }
  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
