namespace PokeGame.Core.Battles;

public interface IStatisticChanges
{
  int Attack { get; }
  int Defense { get; }
  int SpecialAttack { get; }
  int SpecialDefense { get; }
  int Speed { get; }
  int Accuracy { get; }
  int Evasion { get; }
  int Critical { get; }
}
