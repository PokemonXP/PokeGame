using FluentValidation;

namespace PokeGame.Core.Species;

public record EggProperties // TODO(fpion): complete
{
  public EggGroup PrimaryGroup { get; }
  public EggGroup? SecondaryGroup { get; }

  public byte Cycles { get; }

  public EggProperties(EggGroup primaryGroup, EggGroup? secondaryGroup, byte cycles)
  {
    PrimaryGroup = primaryGroup;
    SecondaryGroup = secondaryGroup;
    Cycles = cycles;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<EggProperties>
  {
    public Validator()
    {
      RuleFor(x => x.PrimaryGroup).IsInEnum();
      RuleFor(x => x.SecondaryGroup).IsInEnum().NotEqual(x => x.PrimaryGroup);

      RuleFor(x => x.Cycles).GreaterThan((byte)0);
    }
  }
}
