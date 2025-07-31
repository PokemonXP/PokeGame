namespace PokeGame.Core.Pokemon;

public record PokemonSlot
{
  public Position Position { get; }
  public Box? Box { get; }

  public PokemonSlot(Position position, Box? box = null)
  {
    Position = position;
    Box = box;
    // TODO(fpion): validate
  }
}
