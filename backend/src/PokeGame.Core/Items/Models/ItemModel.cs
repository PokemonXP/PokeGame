using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.Core.Items.Models;

public class ItemModel : AggregateModel
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int Price { get; set; }

  public ItemCategory Category { get; set; }
  public BattleItemModel? BattleItem { get; set; }
  public BerryModel? Berry { get; set; }
  public MedicineModel? Medicine { get; set; }
  public PokeBallModel? PokeBall { get; set; }
  public TechnicalMachineModel? TechnicalMachine { get; set; }

  public string? Sprite { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
