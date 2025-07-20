using FluentValidation;
using PokeGame.Core.Items.Validators;

namespace PokeGame.Core.Items.Properties;

public record PokeBallProperties : ItemProperties, IPokeBallProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.PokeBall;

  public double CatchMultiplier { get; }
  public bool Heal { get; }
  public byte BaseFriendship { get; }
  public double FriendshipMultiplier { get; }

  [JsonConstructor]
  public PokeBallProperties(
    double catchMultiplier = 1.0,
    bool heal = false,
    byte baseFriendship = 0,
    double friendshipMultiplier = 1.0)
  {
    CatchMultiplier = catchMultiplier;
    Heal = heal;
    BaseFriendship = baseFriendship;
    FriendshipMultiplier = friendshipMultiplier;
    new PokeBallValidator().ValidateAndThrow(this);
  }

  public PokeBallProperties(IPokeBallProperties pokeBall) : this(pokeBall.CatchMultiplier, pokeBall.Heal, pokeBall.BaseFriendship, pokeBall.FriendshipMultiplier)
  {
  }
}
