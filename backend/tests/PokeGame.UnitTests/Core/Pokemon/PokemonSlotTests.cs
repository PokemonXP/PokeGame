using FluentValidation;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonSlotTests
{
  [Fact(DisplayName = "IsGreaterThan: it should return false when the other slot is less than or equal to the current slot.")]
  public void Given_NotGreater_When_IsGreaterThan_Then_False()
  {
    PokemonSlot slot1 = new(new Position(1));
    PokemonSlot slot2 = new(new Position(2));
    Assert.False(slot1.IsGreaterThan(slot1));
    Assert.False(slot1.IsGreaterThan(slot2));
  }

  [Fact(DisplayName = "IsGreaterThan: it should return true when the other slot is greater than the current slot.")]
  public void Given_Greater_When_IsGreaterThan_Then_True()
  {
    PokemonSlot slot1 = new(new Position(1));
    PokemonSlot slot2 = new(new Position(2));
    Assert.True(slot2.IsGreaterThan(slot1));
  }

  [Fact(DisplayName = "IsGreaterThan: it should throw ArgumentException when the slots are in different boxes.")]
  public void Given_DifferentBoxes_When_IsGreaterThan_Then_ArgumentException()
  {
    PokemonSlot slot1 = new(new Position(1));
    PokemonSlot slot2 = new(new Position(2), new Box(0));
    var exception = Assert.Throws<ArgumentException>(() => slot1.IsGreaterThan(slot2));
    Assert.Equal("slot", exception.ParamName);
    Assert.StartsWith("Cannot compare slots that are not in the same box/party.", exception.Message);
  }

  [Fact(DisplayName = "IsLesserThan: it should return false when the other slot is greater than or equal to the current slot.")]
  public void Given_NotGreater_When_IsLesserThan_Then_False()
  {
    PokemonSlot slot1 = new(new Position(1));
    PokemonSlot slot2 = new(new Position(2));
    Assert.False(slot1.IsLesserThan(slot1));
    Assert.False(slot2.IsLesserThan(slot1));
  }

  [Fact(DisplayName = "IsLesserThan: it should return true when the other slot is lesser than the current slot.")]
  public void Given_Lesser_When_IsLesserThan_Then_True()
  {
    PokemonSlot slot1 = new(new Position(1));
    PokemonSlot slot2 = new(new Position(2));
    Assert.True(slot1.IsLesserThan(slot2));
  }

  [Fact(DisplayName = "IsLesserThan: it should throw ArgumentException when the slots are in different boxes.")]
  public void Given_DifferentBoxes_When_IsLesserThan_Then_ArgumentException()
  {
    PokemonSlot slot1 = new(new Position(1));
    PokemonSlot slot2 = new(new Position(2), new Box(0));
    var exception = Assert.Throws<ArgumentException>(() => slot1.IsLesserThan(slot2));
    Assert.Equal("slot", exception.ParamName);
    Assert.StartsWith("Cannot compare slots that are not in the same box/party.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ValidationException when the position is not valid.")]
  public void Given_InvalidPosition_When_ctor_Then_ValidationException()
  {
    Position position = new(10);
    var exception = Assert.Throws<ValidationException>(() => new PokemonSlot(position));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.PropertyName == "Position.Value" && e.AttemptedValue?.Equals(position.Value) == true
      && e.ErrorCode == "PositionValidator" && e.ErrorMessage == "The position must be inferior to '6' when the Pokémon is in the party.");
  }

  [Fact(DisplayName = "Next: it should return the next box slot.")]
  public void Given_Box_When_Next_Then_CorrectSlot()
  {
    PokemonSlot slot = new(new Position(3), new Box(3));
    slot = slot.Next();
    Assert.Equal(4, slot.Position.Value);
    Assert.NotNull(slot.Box);
    Assert.Equal(3, slot.Box.Value);

    slot = new(new Position(29), new Box(1));
    slot = slot.Next();
    Assert.Equal(0, slot.Position.Value);
    Assert.NotNull(slot.Box);
    Assert.Equal(2, slot.Box.Value);
  }

  [Fact(DisplayName = "Next: it should return the next party slot.")]
  public void Given_Party_When_Next_Then_CorrectSlot()
  {
    PokemonSlot slot = new(new Position(3));
    slot = slot.Next();
    Assert.Equal(4, slot.Position.Value);
    Assert.Null(slot.Box);
  }

  [Fact(DisplayName = "Next: it should throw InvalidOperationException when in the last box slot.")]
  public void Given_LastBoxSlot_When_Next_Then_InvalidOperationException()
  {
    PokemonSlot slot = new(new Position(29), new Box(31));
    var exception = Assert.Throws<InvalidOperationException>(slot.Next);
    Assert.Equal("The current slot is the last box slot.", exception.Message);
  }

  [Fact(DisplayName = "Next: it should throw InvalidOperationException when in the last party slot.")]
  public void Given_LastPartySlot_When_Next_Then_InvalidOperationException()
  {
    PokemonSlot slot = new(new Position(5));
    var exception = Assert.Throws<InvalidOperationException>(slot.Next);
    Assert.Equal("The current slot is the last party slot.", exception.Message);
  }

  [Fact(DisplayName = "Previous: it should return the previous box slot.")]
  public void Given_Box_When_Previous_Then_CorrectSlot()
  {
    PokemonSlot slot = new(new Position(3), new Box(3));
    slot = slot.Previous();
    Assert.Equal(2, slot.Position.Value);
    Assert.NotNull(slot.Box);
    Assert.Equal(3, slot.Box.Value);

    slot = new(new Position(0), new Box(1));
    slot = slot.Previous();
    Assert.Equal(29, slot.Position.Value);
    Assert.NotNull(slot.Box);
    Assert.Equal(0, slot.Box.Value);
  }

  [Fact(DisplayName = "Previous: it should return the previous party slot.")]
  public void Given_Party_When_Previous_Then_CorrectSlot()
  {
    PokemonSlot slot = new(new Position(3));
    slot = slot.Previous();
    Assert.Equal(2, slot.Position.Value);
    Assert.Null(slot.Box);
  }

  [Fact(DisplayName = "Previous: it should throw InvalidOperationException when in the first box slot.")]
  public void Given_FirstBoxSlot_When_Previous_Then_InvalidOperationException()
  {
    PokemonSlot slot = new(new Position(0), new Box(0));
    var exception = Assert.Throws<InvalidOperationException>(slot.Previous);
    Assert.Equal("The current slot is the first box slot.", exception.Message);
  }

  [Fact(DisplayName = "Previous: it should throw InvalidOperationException when in the first party slot.")]
  public void Given_FirstPartySlot_When_Previous_Then_InvalidOperationException()
  {
    PokemonSlot slot = new(new Position(0));
    var exception = Assert.Throws<InvalidOperationException>(slot.Previous);
    Assert.Equal("The current slot is the first party slot.", exception.Message);
  }
}
