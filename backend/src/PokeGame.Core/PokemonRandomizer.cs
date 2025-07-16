using PokeGame.Core.Forms.Models;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Varieties;

namespace PokeGame.Core;

public interface IPokemonRandomizer
{
  AbilitySlot AbilitySlot(AbilitiesModel abilities);
  IndividualValues IndividualValues();
  PokemonGender PokemonGender(GenderRatio genderRatio);
  PokemonNature PokemonNature();
  PokemonSize PokemonSize();
}

public class PokemonRandomizer : IPokemonRandomizer
{
  private static IPokemonRandomizer? _instance = null;
  public static IPokemonRandomizer Instance
  {
    get
    {
      _instance ??= new PokemonRandomizer();
      return _instance;
    }
  }

  private readonly Random _random = new();

  private PokemonRandomizer()
  {
  }

  public AbilitySlot AbilitySlot(AbilitiesModel abilities)
  {
    List<AbilitySlot> slots = new(capacity: 3)
    {
      Core.AbilitySlot.Primary
    };
    if (abilities.Secondary is not null)
    {
      slots.Add(Core.AbilitySlot.Secondary);
    }
    if (abilities.Hidden is not null)
    {
      slots.Add(Core.AbilitySlot.Hidden);
    }
    return _random.Pick(slots);
  }

  public IndividualValues IndividualValues()
  {
    byte[] bytes = new byte[6];
    for (int i = 0; i < bytes.Length; i++)
    {
      bytes[i] = (byte)_random.Next(0, Pokemons.IndividualValues.MaximumValue + 1);
    }
    return new IndividualValues(bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5]);
  }

  public PokemonGender PokemonGender(GenderRatio genderRatio)
  {
    int value = _random.Next(0, GenderRatio.MaximumValue);
    return value < genderRatio.Value ? Pokemons.PokemonGender.Male : Pokemons.PokemonGender.Female;
  }

  public PokemonNature PokemonNature() => _random.Pick(PokemonNatures.Instance.ToList());

  public PokemonSize PokemonSize()
  {
    int height = _random.Next(0, 128 + 1) + _random.Next(0, 127 + 1);
    int weight = _random.Next(0, 128 + 1) + _random.Next(0, 127 + 1);
    return new PokemonSize((byte)height, (byte)weight);
  }
}
