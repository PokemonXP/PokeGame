using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record PokeBallPropertiesModel : IPokeBallProperties
{
  public double CatchMultiplier { get; set; }
  public bool Heal { get; set; }
  public byte BaseFriendship { get; set; }
  public double FriendshipMultiplier { get; set; }

  public PokeBallPropertiesModel() : this(catchMultiplier: 1.0, heal: false, baseFriendship: 0, friendshipMultiplier: 1.0)
  {
  }

  [JsonConstructor]
  public PokeBallPropertiesModel(double catchMultiplier, bool heal, byte baseFriendship, double friendshipMultiplier)
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
