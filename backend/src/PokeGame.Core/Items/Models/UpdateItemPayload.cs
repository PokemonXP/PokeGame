using Krakenar.Contracts;

namespace PokeGame.Core.Items.Models;

public record UpdateItemPayload
{
  public string? UniqueName { get; set; } = string.Empty;
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public int? Price { get; set; }

  public BattleItemPropertiesModel? BattleItem { get; set; }
  public BerryPropertiesModel? Berry { get; set; }
  public KeyItemPropertiesModel? KeyItem { get; set; }
  public MedicinePropertiesModel? Medicine { get; set; }
  public OtherItemPropertiesModel? OtherItem { get; set; }
  public PicnicItemPropertiesModel? PicnicItem { get; set; }
  public PokeBallPropertiesModel? PokeBall { get; set; }
  public TechnicalMachinePropertiesPayload? TechnicalMachine { get; set; }
  public MaterialPropertiesModel? TechnicalMachineMaterial { get; set; }
  public TreasurePropertiesModel? Treasure { get; set; }

  public Change<string>? Sprite { get; set; }
  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }
}
