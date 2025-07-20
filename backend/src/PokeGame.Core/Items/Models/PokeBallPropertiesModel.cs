using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record PokeBallPropertiesModel : IPokeBallProperties
{
  public double CatchMultiplier { get; set; }
  public bool Heal { get; set; }
  public byte BaseFriendship { get; set; }
  public double FriendshipMultiplier { get; set; }
}
