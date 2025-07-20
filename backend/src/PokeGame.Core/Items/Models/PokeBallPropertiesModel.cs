using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record PokeBallPropertiesModel : IPokeBallProperties
{
  public double CatchMultiplier { get; set; }
  public bool Heal { get; set; }
  public byte BaseFriendship { get; set; }
  public double FriendshipMultiplier { get; set; }

  [JsonConstructor]
  public PokeBallPropertiesModel(
  double catchMultiplier = 1.0,
  bool heal = false,
  byte baseFriendship = 0,
  double friendshipMultiplier = 1.0)
  {
    CatchMultiplier = catchMultiplier;
    Heal = heal;
    BaseFriendship = baseFriendship;
    FriendshipMultiplier = friendshipMultiplier;
  }

  public PokeBallPropertiesModel(IPokeBallProperties pokeBall) : this(pokeBall.CatchMultiplier, pokeBall.Heal, pokeBall.BaseFriendship, pokeBall.FriendshipMultiplier)
  {
  }
}
