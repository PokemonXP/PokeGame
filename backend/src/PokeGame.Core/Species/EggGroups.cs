using FluentValidation;
using PokeGame.Core.Species.Validators;

namespace PokeGame.Core.Species;

public record EggGroups : IEggGroups
{
  public EggGroup Primary { get; }
  public EggGroup? Secondary { get; }

  public EggGroups()
  {
  }

  [JsonConstructor]
  public EggGroups(EggGroup primary, EggGroup? secondary = null)
  {
    Primary = primary;
    Secondary = secondary;
    new EggGroupsValidator().ValidateAndThrow(this);
  }

  public EggGroups(IEggGroups eggs) : this(eggs.Primary, eggs.Secondary)
  {
  }
}
