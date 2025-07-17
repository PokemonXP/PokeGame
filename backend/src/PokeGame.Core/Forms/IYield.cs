namespace PokeGame.Core.Forms;

public interface IYield
{
  int Experience { get; }

  int HP { get; }
  int Attack { get; }
  int Defense { get; }
  int SpecialAttack { get; }
  int SpecialDefense { get; }
  int Speed { get; }
}
