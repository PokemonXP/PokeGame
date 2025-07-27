using FluentValidation;

namespace PokeGame.Core.Pokemon;

public record PokemonSlot
{
  public const int PartySize = 6;

  public Position Position { get; }
  public Box? Box { get; }

  public PokemonSlot(Position position, Box? box = null)
  {
    Position = position;
    Box = box;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<PokemonSlot>
  {
    public Validator()
    {
      When(x => x.Box is null, () => RuleFor(x => x.Position.Value).LessThan(PartySize));
    }
  }
}
