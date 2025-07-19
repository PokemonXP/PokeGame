namespace PokeGame.Core.Pokemon;

public interface IEffortValues
{
  byte HP { get; }
  byte Attack { get; }
  byte Defense { get; }
  byte SpecialAttack { get; }
  byte SpecialDefense { get; }
  byte Speed { get; }
}
