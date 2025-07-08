using Krakenar.Core;
using Krakenar.Core.Settings;
using PokeGame.Core.Forms;
using PokeGame.Core.Species;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonTests
{
  [Fact(DisplayName = "It should calculate the correct experience to next level.")]
  public void Given_Experience_When_ToNextLevel_Then_Correct()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028);
    Assert.Equal(549, pokemon.ToNextLevel);
  }

  [Fact(DisplayName = "It should calculate the correct level.")]
  public void Given_Experience_When_Level_Then_Correct()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028);
    Assert.Equal(21, pokemon.Level);
  }

  [Fact(DisplayName = "It should calculate the correct maximum experience.")]
  public void Given_Experience_When_MaximumExperience_Then_Correct()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028);
    Assert.Equal(7577, pokemon.MaximumExperience);
  }

  [Fact(DisplayName = "It should not create a Pokémon with more Stamina than its maximum.")]
  public void Given_StaminaGreaterThanMaximum_When_ctor_Then_MaximumStamina()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028,
      vitality: 64,
      stamina: 999);
    Assert.Equal(64, pokemon.Vitality);
    Assert.Equal(pokemon.Statistics.HP, pokemon.Stamina);
  }

  [Fact(DisplayName = "It should not create a Pokémon with more Vitality than its maximum.")]
  public void Given_VitalityGreaterThanMaximum_When_ctor_Then_MaximumVitality()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(27, 27, 25, 22, 25, 26),
      new EffortValues(4, 0, 16, 0, 0, 16),
      GrowthRate.MediumSlow,
      experience: 7028,
      vitality: int.MaxValue);
    Assert.Equal(pokemon.Statistics.HP, pokemon.Vitality);
  }

  [Fact(DisplayName = "It should not have the correct flavor preferrences.")]
  public void Given_Nature_When_Flavors_Then_CorrectFlavors()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55));
    Assert.Equal(Flavor.Bitter, pokemon.FavoriteFlavor);
    Assert.Equal(Flavor.Dry, pokemon.DislikedFlavor);
  }
}
