namespace PokeGame.Core.Items.Properties;

public interface IPokeBallProperties
{
  double CatchMultiplier { get; }
  bool Heal { get; }
  byte BaseFriendship { get; }
  double FriendshipMultiplier { get; }
}
