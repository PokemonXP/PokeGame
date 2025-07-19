using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonConstructorTests
{
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;

  public PokemonConstructorTests()
  {
    _species = new PokemonSpecies(new Number(499), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "pignite"), new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow);

    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(7));

    Ability blaze = new(new UniqueName(_uniqueNameSettings, "blaze"));
    Ability thickFat = new(new UniqueName(_uniqueNameSettings, "thick-fat"));
    Sprites sprites = new(new Url("https://www.pokegame.com/assets/img/pokemon/pignite.png"), new Url("https://www.pokegame.com/assets/img/pokemon/pignite-shiny.png"));
    _form = new Form(_variety, _variety.UniqueName, new FormTypes(PokemonType.Fire, PokemonType.Fighting),
      new FormAbilities(blaze, secondary: null, thickFat), new BaseStatistics(90, 93, 55, 70, 55, 55),
      new Yield(146, 0, 2, 0, 0, 0, 0), sprites, isDefault: true, height: new Height(10), weight: new Weight(555));
  }

  [Fact(DisplayName = "It should create an egg Pokémon.")]
  public void Given_EggCycles_When_ctor_Then_EggPokemon()
  {
    Specimen pokemon = new(
      _species,
      _variety,
      _form,
      _form.UniqueName,
      _randomizer.PokemonSize(),
      _randomizer.PokemonNature(),
      _randomizer.IndividualValues(),
      PokemonGender.Male,
      eggCycles: _species.EggCycles);

    Assert.False(pokemon.IsShiny);
    Assert.Equal(PokemonType.Fire, pokemon.TeraType);
    Assert.Equal(AbilitySlot.Primary, pokemon.AbilitySlot);

    Assert.Equal(_species.EggCycles, pokemon.EggCycles);
    Assert.Equal(_species.GrowthRate, pokemon.GrowthRate);
    Assert.Equal(1, pokemon.Level);
    Assert.Equal(0, pokemon.Experience);

    Assert.Equal(pokemon.Statistics.HP, pokemon.Vitality);
    Assert.Equal(pokemon.Statistics.HP, pokemon.Stamina);
    Assert.Equal(_species.BaseFriendship, pokemon.Friendship);

    Assert.Equal(0, pokemon.EffortValues.HP);
    Assert.Equal(0, pokemon.EffortValues.Attack);
    Assert.Equal(0, pokemon.EffortValues.Defense);
    Assert.Equal(0, pokemon.EffortValues.SpecialAttack);
    Assert.Equal(0, pokemon.EffortValues.SpecialDefense);
    Assert.Equal(0, pokemon.EffortValues.Speed);
  }

  [Theory(DisplayName = "It should create the correct Pokémon from arguments.")]
  [InlineData(false, 10, 5)]
  [InlineData(true, 999, 999)]
  public void Given_Arguments_When_ctor_Then_CorrectPokemon(bool isShiny, int vitality, int stamina)
  {
    ActorId actorId = ActorId.NewId();
    PokemonId pokemonId = PokemonId.NewId();

    UniqueName uniqueName = new(_uniqueNameSettings, "briquet");
    PokemonSize size = new(128, 128);
    PokemonNature nature = PokemonNatures.Instance.Find("careful");
    IndividualValues individualValues = new(27, 27, 25, 22, 25, 26);
    PokemonGender gender = PokemonGender.Male;
    PokemonType teraType = PokemonType.Fighting;
    AbilitySlot abilitySlot = AbilitySlot.Hidden;
    int experience = 7028;
    EffortValues effortValues = new(4, 0, 16, 0, 0, 16);
    Friendship friendship = new(91);

    Specimen pokemon = new(
      _species,
      _variety,
      _form,
      uniqueName,
      size,
      nature,
      individualValues,
      gender,
      isShiny,
      teraType,
      abilitySlot,
      eggCycles: null,
      experience,
      effortValues,
      vitality,
      stamina,
      friendship,
      actorId,
      pokemonId);

    Assert.Equal(pokemonId, pokemon.Id);
    Assert.Equal(actorId, pokemon.CreatedBy);
    Assert.Equal(actorId, pokemon.UpdatedBy);

    Assert.Equal(_species.Id, pokemon.SpeciesId);
    Assert.Equal(_variety.Id, pokemon.VarietyId);
    Assert.Equal(_form.Id, pokemon.FormId);

    Assert.Equal(uniqueName, pokemon.UniqueName);
    Assert.Equal(gender, pokemon.Gender);
    Assert.Equal(isShiny, pokemon.IsShiny);
    Assert.Equal(teraType, pokemon.TeraType);
    Assert.Equal(size, pokemon.Size);
    Assert.Equal(abilitySlot, pokemon.AbilitySlot);
    Assert.Equal(nature, pokemon.Nature);

    Assert.Null(pokemon.EggCycles);
    Assert.Equal(_species.GrowthRate, pokemon.GrowthRate);
    Assert.Equal(21, pokemon.Level);
    Assert.Equal(experience, pokemon.Experience);

    PokemonStatistics statistics = new(_form.BaseStatistics, individualValues, effortValues, level: 21, nature);
    Assert.Equal(_form.BaseStatistics, pokemon.BaseStatistics);
    Assert.Equal(individualValues, pokemon.IndividualValues);
    Assert.Equal(effortValues, pokemon.EffortValues);
    Assert.Equal(statistics, pokemon.Statistics);
    Assert.Equal(Math.Min(vitality, statistics.HP), pokemon.Vitality);
    Assert.Equal(Math.Min(stamina, statistics.HP), pokemon.Stamina);
    Assert.Null(pokemon.StatusCondition);
    Assert.Equal(friendship, pokemon.Friendship);

    PokemonCharacteristic characteristic = PokemonCharacteristics.Instance.Find(individualValues, size);
    Assert.Equal(characteristic, pokemon.Characteristic);
  }

  [Fact(DisplayName = "It should create the correct Pokémon from default arguments.")]
  public void Given_DefaultArguments_When_ctor_Then_CorrectPokemon()
  {
    Specimen pokemon = new(
      _species,
      _variety,
      _form,
      _form.UniqueName,
      _randomizer.PokemonSize(),
      _randomizer.PokemonNature(),
      _randomizer.IndividualValues(),
      PokemonGender.Male);

    Assert.False(pokemon.IsShiny);
    Assert.Equal(PokemonType.Fire, pokemon.TeraType);
    Assert.Equal(AbilitySlot.Primary, pokemon.AbilitySlot);

    Assert.Equal(_species.GrowthRate, pokemon.GrowthRate);
    Assert.Equal(1, pokemon.Level);
    Assert.Equal(0, pokemon.Experience);

    Assert.Equal(pokemon.Statistics.HP, pokemon.Vitality);
    Assert.Equal(pokemon.Statistics.HP, pokemon.Stamina);
    Assert.Equal(_species.BaseFriendship, pokemon.Friendship);

    Assert.Equal(0, pokemon.EffortValues.HP);
    Assert.Equal(0, pokemon.EffortValues.Attack);
    Assert.Equal(0, pokemon.EffortValues.Defense);
    Assert.Equal(0, pokemon.EffortValues.SpecialAttack);
    Assert.Equal(0, pokemon.EffortValues.SpecialDefense);
    Assert.Equal(0, pokemon.EffortValues.Speed);
  }

  [Fact(DisplayName = "It should throw ArgumentException when an egg Pokémon has experience.")]
  public void Given_EggCyclesWithExperience_When_ctor_Then_ArgumentException()
  {
    var exception = Assert.Throws<ArgumentException>(
      () => new Specimen(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male, eggCycles: _species.EggCycles, experience: 10));
    Assert.Equal("experience", exception.ParamName);
    Assert.StartsWith("An egg Pokémon cannot have experience.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the ability slot is not valid.")]
  public void Given_InvalidAbilitySlot_When_ctor_Then_ArgumentException()
  {
    var exception = Assert.Throws<ArgumentException>(
      () => new Specimen(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male, abilitySlot: AbilitySlot.Secondary));
    Assert.Equal("abilitySlot", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the form is not valid.")]
  public void Given_InvalidForm_When_ctor_Then_ArgumentException()
  {
    PokemonSpecies species = new(new Number(498), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "tepig"), new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow);
    Variety variety = new(species, species.UniqueName, isDefault: true, new GenderRatio(7));
    Sprites sprites = new(new Url("https://www.pokegame.com/assets/img/pokemon/tepig.png"), new Url("https://www.pokegame.com/assets/img/pokemon/tepig-shiny.png"));
    Form form = new(variety, variety.UniqueName, new FormTypes(PokemonType.Fire), _form.Abilities, new BaseStatistics(65, 63, 45, 45, 45, 45),
      new Yield(62, 1, 0, 0, 0, 0, 0), sprites, isDefault: true, height: new Height(5), weight: new Weight(99));

    var exception = Assert.Throws<ArgumentException>(
      () => new Specimen(_species, _variety, form, new(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues()));
    Assert.Equal("form", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the gender should be unknown.")]
  public void Given_GenderShouldBeUnknown_When_ctor_Then_ArgumentException()
  {
    PokemonSpecies species = new(new Number(132), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "ditto"));
    Variety variety = new(species, species.UniqueName, isDefault: true);

    Ability limber = new(new UniqueName(_uniqueNameSettings, "limber"));
    Ability imposter = new(new UniqueName(_uniqueNameSettings, "imposter"));
    Sprites sprites = new(new Url("https://www.pokegame.com/assets/img/pokemon/ditto.png"), new Url("https://www.pokegame.com/assets/img/pokemon/ditto-shiny.png"));
    Form form = new(variety, variety.UniqueName, new FormTypes(PokemonType.Normal), new FormAbilities(limber, imposter), new BaseStatistics(48, 48, 48, 48, 48, 48),
      new Yield(101, 1, 0, 0, 0, 0, 0), sprites, isDefault: true, height: new Height(3), weight: new Weight(40));

    var exception = Assert.Throws<ArgumentException>(
      () => new Specimen(species, variety, form, form.UniqueName, _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male));
    Assert.Equal("gender", exception.ParamName);
    Assert.StartsWith("The Pokémon should not have a gender (unknown).", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the variety is not valid.")]
  public void Given_InvalidVariety_When_ctor_Then_ArgumentException()
  {
    PokemonSpecies species = new(new Number(498), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "tepig"), new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow);
    Variety variety = new(species, species.UniqueName, isDefault: true, new GenderRatio(7));

    var exception = Assert.Throws<ArgumentException>(
      () => new Specimen(_species, variety, _form, new(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues()));
    Assert.Equal("variety", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentNullException when the gender is missing.")]
  public void Given_GenderIsMissing_When_ctor_Then_ArgumentNullException()
  {
    var exception = Assert.Throws<ArgumentNullException>(
      () => new Specimen(_species, _variety, _form, new(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues()));
    Assert.Equal("gender", exception.ParamName);
    Assert.StartsWith("The Pokémon should have a gender.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the ability slot is not defined.")]
  public void Given_AbilitySlotNotDefined_When_ctor_Then_ArgumentOutOfRangeException()
  {
    AbilitySlot abilitySlot = (AbilitySlot)(-1);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Specimen(_species, _variety, _form, new(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male, abilitySlot: abilitySlot));
    Assert.Equal("abilitySlot", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the egg cycles exceed species cycles.")]
  public void Given_EggCyclesExceedSpecies_When_ctor_Then_ArgumentOutOfRangeException()
  {
    EggCycles eggCycles = new((byte)(_species.EggCycles.Value * 2));
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Specimen(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male, eggCycles: eggCycles));
    Assert.Equal("eggCycles", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the experience is negative.")]
  public void Given_ExperienceIsNegative_When_ctor_Then_ArgumentOutOfRangeException()
  {
    int experience = -100;
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Specimen(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male, experience: experience));
    Assert.Equal("experience", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the gender is not defined.")]
  public void Given_GenderNotDefined_When_ctor_Then_ArgumentOutOfRangeException()
  {
    PokemonGender gender = (PokemonGender)(-1);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Specimen(_species, _variety, _form, new(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), gender));
    Assert.Equal("gender", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the gender is not Female.")]
  public void Given_GenderNotFemale_When_ctor_Then_ArgumentOutOfRangeException()
  {
    PokemonSpecies species = new(new Number(241), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "miltank"));
    Variety variety = new(species, species.UniqueName, isDefault: true, new GenderRatio(0));

    Ability thickFat = new(new UniqueName(_uniqueNameSettings, "thick-fat"));
    Ability scrappy = new(new UniqueName(_uniqueNameSettings, "scrappy"));
    Ability sapSipper = new(new UniqueName(_uniqueNameSettings, "sap-sipper"));
    Sprites sprites = new(new Url("https://www.pokegame.com/assets/img/pokemon/miltank.png"), new Url("https://www.pokegame.com/assets/img/pokemon/miltank-shiny.png"));
    Form form = new(variety, variety.UniqueName, new FormTypes(PokemonType.Normal), new FormAbilities(thickFat, scrappy, sapSipper), new BaseStatistics(95, 80, 105, 40, 70, 100),
      new Yield(172, 0, 0, 2, 0, 0, 0), sprites, isDefault: true, height: new Height(12), weight: new Weight(755));

    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Specimen(species, variety, form, form.UniqueName, _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male));
    Assert.Equal("gender", exception.ParamName);
    Assert.StartsWith("The gender 'Male' is not valid for ratio '0'.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the gender is not Male.")]
  public void Given_GenderNotMale_When_ctor_Then_ArgumentOutOfRangeException()
  {
    PokemonSpecies species = new(new Number(128), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "tauros"));
    Variety variety = new(species, species.UniqueName, isDefault: true, new GenderRatio(8));

    Ability intimidate = new(new UniqueName(_uniqueNameSettings, "intimidate"));
    Ability angerPoint = new(new UniqueName(_uniqueNameSettings, "anger-point"));
    Ability sheerForce = new(new UniqueName(_uniqueNameSettings, "sheer-force"));
    Sprites sprites = new(new Url("https://www.pokegame.com/assets/img/pokemon/tauros.png"), new Url("https://www.pokegame.com/assets/img/pokemon/tauros-shiny.png"));
    Form form = new(variety, variety.UniqueName, new FormTypes(PokemonType.Normal), new FormAbilities(intimidate, angerPoint, sheerForce), new BaseStatistics(75, 100, 95, 40, 70, 110),
      new Yield(172, 0, 1, 0, 0, 0, 1), sprites, isDefault: true, height: new Height(14), weight: new Weight(884));

    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Specimen(species, variety, form, form.UniqueName, _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Female));
    Assert.Equal("gender", exception.ParamName);
    Assert.StartsWith("The gender 'Female' is not valid for ratio '8'.", exception.Message);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the stamina is negative.")]
  public void Given_StaminaIsNegative_When_ctor_Then_ArgumentOutOfRangeException()
  {
    int stamina = -10;
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Specimen(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male, stamina: stamina));
    Assert.Equal("stamina", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the Tera type is not defined.")]
  public void Given_TeraTypeNotDefined_When_ctor_Then_ArgumentOutOfRangeException()
  {
    PokemonType teraType = (PokemonType)(-1);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Specimen(_species, _variety, _form, new(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male, teraType: teraType));
    Assert.Equal("teraType", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the vitality is negative.")]
  public void Given_VitalityIsNegative_When_ctor_Then_ArgumentOutOfRangeException()
  {
    int vitality = -1;
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Specimen(_species, _variety, _form, new UniqueName(_uniqueNameSettings, "briquet"), _randomizer.PokemonSize(), _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male, vitality: vitality));
    Assert.Equal("vitality", exception.ParamName);
  }
}
