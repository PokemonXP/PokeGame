namespace PokeGame.Core.Items.Models;

public record MedicineModel
{
  public bool IsHerbal { get; set; }

  public HealingModel? Healing { get; set; }
  public StatusHealingModel? Status { get; set; }
  public PowerPointRestoreModel? PowerPoints { get; set; }
}
