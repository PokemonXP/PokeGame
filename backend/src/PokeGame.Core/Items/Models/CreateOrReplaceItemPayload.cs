namespace PokeGame.Core.Items.Models;

public record CreateOrReplaceItemPayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int? Price { get; set; }

  public BattleItemPropertiesModel? BattleItem { get; set; }
  public BerryPropertiesModel? Berry { get; set; }
  public KeyItemPropertiesModel? KeyItem { get; set; }
  public MedicinePropertiesModel? Medicine { get; set; }
  public OtherItemPropertiesModel? OtherItem { get; set; }
  public PicnicItemPropertiesModel? PicnicItem { get; set; }
  public PokeBallPropertiesModel? PokeBall { get; set; }
  public TechnicalMachinePropertiesPayload? TechnicalMachine { get; set; }
  public TechnicalMachineMaterialPropertiesModel? TechnicalMachineMaterial { get; set; }
  public TreasurePropertiesModel? Treasure { get; set; }

  public string? Sprite { get; set; }
  public string? Url { get; set; }
  public string? Notes { get; set; }
}
