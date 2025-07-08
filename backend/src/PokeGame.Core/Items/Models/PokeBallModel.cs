namespace PokeGame.Core.Items.Models;

public record PokeBallModel
{
  public double CatchMultiplier { get; set; }
  public bool Heal { get; set; }
  public int BaseFriendship { get; set; }
  public int FriendshipMultiplier { get; set; }

  public PokeBallModel()
  {
  }

  public PokeBallModel(double catchMultiplier, bool heal, int baseFriendship, int friendshipMultiplier)
  {
    CatchMultiplier = catchMultiplier;
    Heal = heal;
    FriendshipMultiplier = friendshipMultiplier;
    BaseFriendship = baseFriendship;
  }
}
