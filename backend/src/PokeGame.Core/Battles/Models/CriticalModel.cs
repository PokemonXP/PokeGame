namespace PokeGame.Core.Battles.Models;

public record CriticalModel
{
  private const int MinimumChance = 5;
  private const int MaximumChance = 100;

  public int Stages { get; set; }
  public int Chance { get; set; }

  public CriticalModel()
  {
  }

  public CriticalModel(int stages)
  {
    Stages = stages;

    switch (stages)
    {
      case 0:
        Chance = 5;
        break;
      case 1:
        Chance = 10;
        break;
      case 2:
        Chance = 25;
        break;
      case 3:
        Chance = 50;
        break;
      case 4:
        Chance = 100;
        break;
      default:
        if (stages < 0)
        {
          Chance = MinimumChance;
        }
        else if (stages > 4)
        {
          Chance = MaximumChance;
        }
        break;
    }
  }
}
