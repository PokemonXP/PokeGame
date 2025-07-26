using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonFormTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Ability _zenMode;
  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;

  public PokemonFormTests()
  {
    _zenMode = new(new UniqueName(_uniqueNameSettings, "zen-mode"));

    _species = new PokemonSpecies(new Number(555), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "darmanitan"),
      new Friendship(70), new CatchRate(60), GrowthRate.MediumSlow, new EggCycles(20), new EggGroups(EggGroup.Field));

    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(4), canChangeForm: true);

    Ability sheerForce = new(new UniqueName(_uniqueNameSettings, "sheer-force"));
    _form = new Form(
      _variety,
      _variety.UniqueName,
      new FormTypes(PokemonType.Fire),
      new FormAbilities(sheerForce, secondary: null, _zenMode),
      new BaseStatistics(105, 140, 55, 30, 55, 95),
      new Yield(168, 0, 2, 0, 0, 0, 0),
      new Sprites(
        new Url("https://www.pokegame.com/assets/img/pokemon/darmanitan.png"),
        new Url("https://www.pokegame.com/assets/img/pokemon/darmanitan-shiny.png")),
      isDefault: true,
      isBattleOnly: false,
      isMega: false,
      new Height(13),
      new Weight(929));

    _pokemon = new Specimen(_species, _variety, _form, _species.UniqueName, _randomizer.PokemonSize(), _randomizer.PokemonNature(),
      _randomizer.IndividualValues(), _randomizer.PokemonGender(_variety.GenderRatio!), abilitySlot: AbilitySlot.Hidden);
  }

  [Fact(DisplayName = "ChangeForm: it should change the Pokémon form.")]
  public void Given_ValidForm_When_ChangeForm_Then_FormChanged()
  {
    Form zenForm = new(
      _variety,
      new UniqueName(_uniqueNameSettings, "darmanitan-zen"),
      new FormTypes(PokemonType.Fire, PokemonType.Psychic),
      _form.Abilities,
      new BaseStatistics(105, 30, 105, 140, 105, 55),
      _form.Yield,
      new Sprites(
        new Url("https://www.pokegame.com/assets/img/pokemon/darmanitan-zen.png"),
        new Url("https://www.pokegame.com/assets/img/pokemon/darmanitan-zen-shiny.png")),
      isBattleOnly: true,
      height: _form.Height,
      weight: _form.Weight);

    _pokemon.ChangeForm(zenForm, _actorId);
    Assert.Equal(zenForm.Id, _pokemon.FormId);
    Assert.Equal(zenForm.BaseStatistics, _pokemon.BaseStatistics);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonFormChanged changed && changed.ActorId == _actorId);

    _pokemon.ClearChanges();
    _pokemon.ChangeForm(zenForm);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "ChangeForm: it should throw ArgumentException when the varieties are not the same.")]
  public void Given_DifferentVarieties_When_ChangeForm_Then_ArgumentException()
  {
    Variety galarianVariety = new(_species, new UniqueName(_uniqueNameSettings, "darmanitan-galar"), isDefault: false, _variety.GenderRatio, canChangeForm: true);

    Ability gorillaTactics = new(new UniqueName(_uniqueNameSettings, "gorilla-tactics"));
    Form zenForm = new(
      galarianVariety,
      new UniqueName(_uniqueNameSettings, "darmanitan-galar-zen"),
      new FormTypes(PokemonType.Ice, PokemonType.Fire),
      new FormAbilities(gorillaTactics, secondary: null, _zenMode),
      new BaseStatistics(105, 160, 55, 30, 55, 135),
      new Yield(168, 0, 0, 0, 2, 0, 0),
      new Sprites(
        new Url("https://www.pokegame.com/assets/img/pokemon/darmanitan-galar-zen.png"),
        new Url("https://www.pokegame.com/assets/img/pokemon/darmanitan-galar-zen-shiny.png")),
      isBattleOnly: true,
      height: new Height(17),
      weight: new Weight(1200));

    var exception = Assert.Throws<ArgumentException>(() => _pokemon.ChangeForm(zenForm));
    Assert.Equal("form", exception.ParamName);
    Assert.StartsWith("The Pokémon current and target forms must be from the same variety.", exception.Message);
  }
}
