namespace PokeGame.Core.Pokemons.Models;

public record IndividualValuesModel : IIndividualValues
{
  public byte HP { get; set; }
  public byte Attack { get; set; }
  public byte Defense { get; set; }
  public byte SpecialAttack { get; set; }
  public byte SpecialDefense { get; set; }
  public byte Speed { get; set; }

  public IndividualValuesModel()
  {
  }

  public IndividualValuesModel(byte hp, byte attack, byte defense, byte specialAttack, byte specialDefense, byte speed)
  {
    HP = hp;
    Attack = attack;
    Defense = defense;
    SpecialAttack = specialAttack;
    SpecialDefense = specialDefense;
    Speed = speed;
  }

  public IndividualValuesModel(IIndividualValues iv) : this(iv.HP, iv.Attack, iv.Defense, iv.SpecialAttack, iv.SpecialDefense, iv.Speed)
  {
  }
}
