using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record BattleItemPropertiesModel : IBattleItemProperties
{
  public int Attack { get; set; }
  public int Defense { get; set; }
  public int SpecialAttack { get; set; }
  public int SpecialDefense { get; set; }
  public int Speed { get; set; }
  public int Accuracy { get; set; }
  public int Evasion { get; set; }
  public int Critical { get; set; }
  public int GuardTurns { get; set; }
}
