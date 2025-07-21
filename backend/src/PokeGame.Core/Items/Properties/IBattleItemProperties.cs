namespace PokeGame.Core.Items.Properties;

public interface IBattleItemProperties
{
  int Attack { get; }
  int Defense { get; }
  int SpecialAttack { get; }
  int SpecialDefense { get; }
  int Speed { get; }
  int Accuracy { get; }
  int Evasion { get; }
  int Critical { get; }
  int GuardTurns { get; }
}
