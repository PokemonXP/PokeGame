namespace PokeGame.Core.Species.Models;

public record EggGroupsModel : IEggGroups
{
  public EggGroup Primary { get; set; }
  public EggGroup? Secondary { get; set; }

  public EggGroupsModel()
  {
  }

  public EggGroupsModel(EggGroup primary, EggGroup? secondary = null)
  {
    Primary = primary;
    Secondary = secondary;
  }

  public EggGroupsModel(IEggGroups eggs) : this(eggs.Primary, eggs.Secondary)
  {
  }
}
