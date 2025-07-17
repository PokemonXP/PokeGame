namespace PokeGame.Core.Species;

public interface IEggGroups
{
  EggGroup Primary { get; }
  EggGroup? Secondary { get; }
}
