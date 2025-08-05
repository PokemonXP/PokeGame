using FluentValidation;

namespace PokeGame.Core.Pokemon;

public record PokemonSlot
{
  public const int BoxCount = 32;
  public const int BoxSize = 5 * 6;
  public const int PartySize = 6;

  public Position Position { get; }
  public Box? Box { get; }

  public PokemonSlot(Position position, Box? box = null)
  {
    Position = position;
    Box = box;
    new Validator().ValidateAndThrow(this);
  }

  public bool IsGreaterThan(PokemonSlot slot)
  {
    if (Box != slot.Box)
    {
      throw new ArgumentException("Cannot compare slots that are not in the same box/party.", nameof(slot));
    }
    return Position.Value > slot.Position.Value;
  }

  public bool IsLesserThan(PokemonSlot slot)
  {
    if (Box != slot.Box)
    {
      throw new ArgumentException("Cannot compare slots that are not in the same box/party.", nameof(slot));
    }
    return Position.Value < slot.Position.Value;
  }

  public PokemonSlot Next()
  {
    if (Box is null)
    {
      if (Position.Value == (PartySize - 1))
      {
        throw new InvalidOperationException("The current slot is the last party slot.");
      }
      return new PokemonSlot(new Position(Position.Value + 1));
    }
    else if (Position.Value == (BoxSize - 1))
    {
      if (Box.Value == (BoxCount - 1))
      {
        throw new InvalidOperationException("The current slot is the last box slot.");
      }
      return new PokemonSlot(new Position(0), new Box(Box.Value + 1));
    }
    return new PokemonSlot(new Position(Position.Value + 1), Box);
  }

  public PokemonSlot Previous()
  {
    if (Box is null)
    {
      if (Position.Value == 0)
      {
        throw new InvalidOperationException("The current slot is the first party slot.");
      }
      return new PokemonSlot(new Position(Position.Value - 1));
    }
    else if (Position.Value == 0)
    {
      if (Box.Value == 0)
      {
        throw new InvalidOperationException("The current slot is the first box slot.");
      }
      return new PokemonSlot(new Position(BoxSize - 1), new Box(Box.Value - 1));
    }
    return new PokemonSlot(new Position(Position.Value - 1), Box);
  }

  private class Validator : AbstractValidator<PokemonSlot>
  {
    public Validator()
    {
      When(x => x.Box is null, () => RuleFor(x => x.Position.Value).LessThan(PartySize)
        .WithErrorCode("PositionValidator")
        .WithMessage($"The position must be inferior to '{PartySize}' when the Pokémon is in the party."));
    }
  }
}
