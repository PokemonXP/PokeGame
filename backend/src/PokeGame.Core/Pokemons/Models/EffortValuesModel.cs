namespace PokeGame.Core.Pokemons.Models;

public record EffortValuesModel : IEffortValues
{
  public byte HP { get; set; }
  public byte Attack { get; set; }
  public byte Defense { get; set; }
  public byte SpecialAttack { get; set; }
  public byte SpecialDefense { get; set; }
  public byte Speed { get; set; }

  public EffortValuesModel()
  {
  }

  public EffortValuesModel(byte hp, byte attack, byte defense, byte specialAttack, byte specialDefense, byte speed)
  {
    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
  }

  public EffortValuesModel(IEffortValues ev) : this(ev.HP, ev.Attack, ev.Defense, ev.SpecialAttack, ev.SpecialDefense, ev.Speed)
  {
  }
}
