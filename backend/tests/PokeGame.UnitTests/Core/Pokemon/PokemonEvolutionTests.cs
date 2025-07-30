using FluentValidation;
using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Properties;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonEvolutionTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _rowletSpecies;
  private readonly PokemonSpecies _dartrixSpecies;

  private readonly Variety _rowletVariety;
  private readonly Variety _dartrixVariety;

  private readonly Form _rowletForm;
  private readonly Form _dartrixForm;

  private readonly Evolution _rowletEvolution;

  private readonly Specimen _rowlet;

  public PokemonEvolutionTests()
  {
    _rowletSpecies = new(new Number(722), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "rowlet"),
      new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow, new EggCycles(15), new EggGroups(EggGroup.Flying));
    _dartrixSpecies = new(new Number(723), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "dartrix"),
      new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow, new EggCycles(15), new EggGroups(EggGroup.Flying));

    _rowletVariety = new(_rowletSpecies, _rowletSpecies.UniqueName, isDefault: true, new GenderRatio(7));
    _dartrixVariety = new(_dartrixSpecies, _dartrixSpecies.UniqueName, isDefault: true, new GenderRatio(7));

    Ability overgrow = new(new UniqueName(_uniqueNameSettings, "overgrow"));
    Ability longReach = new(new UniqueName(_uniqueNameSettings, "long-reach"));
    _rowletForm = new(
      _rowletVariety,
      _rowletVariety.UniqueName,
      new FormTypes(PokemonType.Grass, PokemonType.Flying),
      new FormAbilities(overgrow, secondary: null, longReach),
      new BaseStatistics(68, 55, 55, 50, 50, 42),
      new Yield(64, 1, 0, 0, 0, 0, 0),
      new Sprites(
        new Url("https://www.pokegame.com/assets/img/pokemon/rowlet.png"),
        new Url("https://www.pokegame.com/assets/img/pokemon/rowlet-shiny.png")),
      isDefault: true,
      height: new Height(3),
      weight: new Weight(15));
    _dartrixForm = new(
      _dartrixVariety,
      _dartrixVariety.UniqueName,
      new FormTypes(PokemonType.Grass, PokemonType.Flying),
      new FormAbilities(overgrow, secondary: null, longReach),
      new BaseStatistics(78, 75, 75, 70, 70, 52),
      new Yield(147, 2, 0, 0, 0, 0, 0),
      new Sprites(
        new Url("https://www.pokegame.com/assets/img/pokemon/dartrix.png"),
        new Url("https://www.pokegame.com/assets/img/pokemon/dartrix-shiny.png")),
      isDefault: true,
      height: new Height(7),
      weight: new Weight(160));

    _rowletEvolution = new Evolution(_rowletForm, _dartrixForm)
    {
      Level = new Level(17)
    };

    _rowlet = new Specimen(_rowletSpecies, _rowletVariety, _rowletForm, _rowletSpecies.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(_rowletVariety.GenderRatio!));
  }

  [Fact(DisplayName = "Evolve: it should evolve a Pokémon with the required friendship and moves.")]
  public void Given_FriendshipAndMoves_When_Evolve_Then_Evolved()
  {
    PokemonSpecies eeveeSpecies = new(new Number(133), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "eevee"));
    PokemonSpecies sylveonSpecies = new(new Number(700), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "sylveon"));

    Variety eeveeVariety = new(eeveeSpecies, eeveeSpecies.UniqueName, isDefault: true, new GenderRatio(7));
    Variety sylveonVariety = new(sylveonSpecies, sylveonSpecies.UniqueName, isDefault: true, new GenderRatio(7));

    Ability adaptability = new(new UniqueName(_uniqueNameSettings, "adaptability"));
    Form eeveeForm = new(eeveeVariety, eeveeVariety.UniqueName, new FormTypes(PokemonType.Normal),
      new FormAbilities(adaptability), new BaseStatistics(55, 55, 50, 45, 65, 55), new Yield(65, 0, 0, 0, 0, 1, 0),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/eevee.png"), new Url("https://www.pokegame.com/assets/img/pokemon/eevee-shiny.png")));
    Form sylveonForm = new(sylveonVariety, sylveonVariety.UniqueName, new FormTypes(PokemonType.Fairy),
      new FormAbilities(adaptability), new BaseStatistics(95, 65, 65, 110, 130, 60), new Yield(184, 0, 0, 0, 0, 2, 0),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/sylveon.png"), new Url("https://www.pokegame.com/assets/img/pokemon/sylveon-shiny.png")));

    Move babyDollEyes = new(PokemonType.Fairy, MoveCategory.Status, new UniqueName(_uniqueNameSettings, "baby-doll-eyes"), new Accuracy(100), power: null, new PowerPoints(30));
    Evolution evolution = new(eeveeForm, sylveonForm)
    {
      Friendship = true,
      KnownMoveId = babyDollEyes.Id
    };

    Specimen pokemon = new(eeveeSpecies, eeveeVariety, eeveeForm, eeveeSpecies.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(eeveeVariety.GenderRatio!), experience: 4096)
    {
      Friendship = new Friendship(Specimen.EvolutionFriendship)
    };
    pokemon.LearnMove(babyDollEyes);
    PokemonStatistics statistics = pokemon.Statistics;

    pokemon.Evolve(sylveonSpecies, sylveonVariety, sylveonForm, evolution, _actorId);
    Assert.Equal(sylveonSpecies.Id, pokemon.SpeciesId);
    Assert.Equal(sylveonVariety.Id, pokemon.VarietyId);
    Assert.Equal(sylveonForm.Id, pokemon.FormId);
    Assert.Equal(sylveonForm.BaseStatistics, pokemon.BaseStatistics);
    Assert.NotEqual(statistics, pokemon.Statistics);
    Assert.True(pokemon.HasChanges);
    Assert.Contains(pokemon.Changes, change => change is PokemonEvolved evolved && evolved.ActorId == _actorId);
  }

  [Fact(DisplayName = "Evolve: it should evolve a Pokémon with the required level and gender.")]
  public void Given_LevelAndGender_When_Evolve_Then_Evolved()
  {
    PokemonSpecies combeeSpecies = new(new Number(415), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "combee"), growthRate: GrowthRate.MediumSlow);
    PokemonSpecies vespiquenSpecies = new(new Number(416), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "vespiquen"), growthRate: GrowthRate.MediumSlow);

    Variety combeeVariety = new(combeeSpecies, combeeSpecies.UniqueName, isDefault: true, new GenderRatio(7));
    Variety vespiquenVariety = new(vespiquenSpecies, vespiquenSpecies.UniqueName, isDefault: true, new GenderRatio(0));

    Ability honeyGather = new(new UniqueName(_uniqueNameSettings, "honey-gather"));
    Form combeeForm = new(combeeVariety, combeeVariety.UniqueName, new FormTypes(PokemonType.Bug, PokemonType.Flying),
      new FormAbilities(honeyGather), new BaseStatistics(30, 30, 42, 30, 42, 70), new Yield(49, 0, 0, 0, 0, 0, 1),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/combee.png"), new Url("https://www.pokegame.com/assets/img/pokemon/combee-shiny.png")));
    Form vespiquenForm = new(vespiquenVariety, vespiquenVariety.UniqueName, new FormTypes(PokemonType.Bug, PokemonType.Flying),
      new FormAbilities(honeyGather), new BaseStatistics(70, 80, 102, 80, 102, 40), new Yield(166, 0, 0, 1, 0, 1, 0),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/vespiquen.png"), new Url("https://www.pokegame.com/assets/img/pokemon/vespiquen-shiny.png")));

    Evolution evolution = new(combeeForm, vespiquenForm)
    {
      Level = new Level(21),
      Gender = PokemonGender.Female
    };

    Specimen pokemon = new(combeeSpecies, combeeVariety, combeeForm, combeeSpecies.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Female, experience: 6458);
    PokemonStatistics statistics = pokemon.Statistics;

    pokemon.Evolve(vespiquenSpecies, vespiquenVariety, vespiquenForm, evolution, _actorId);
    Assert.Equal(vespiquenSpecies.Id, pokemon.SpeciesId);
    Assert.Equal(vespiquenVariety.Id, pokemon.VarietyId);
    Assert.Equal(vespiquenForm.Id, pokemon.FormId);
    Assert.Equal(vespiquenForm.BaseStatistics, pokemon.BaseStatistics);
    Assert.NotEqual(statistics, pokemon.Statistics);
    Assert.True(pokemon.HasChanges);
    Assert.Contains(pokemon.Changes, change => change is PokemonEvolved evolved && evolved.ActorId == _actorId);
  }

  [Fact(DisplayName = "Evolve: it should evolve a traded Pokémon holding the right item.")]
  public void Given_TradedPokemon_When_Evolve_Then_Evolved()
  {
    PokemonSpecies scytherSpecies = new(new Number(123), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "scyther"));
    PokemonSpecies scizorSpecies = new(new Number(212), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "scizor"));

    Variety scytherVariety = new(scytherSpecies, scytherSpecies.UniqueName, isDefault: true, new GenderRatio(4));
    Variety scizorVariety = new(scizorSpecies, scizorSpecies.UniqueName, isDefault: true, new GenderRatio(4));

    Ability swarm = new(new UniqueName(_uniqueNameSettings, "swarm"));
    Form scytherForm = new(scytherVariety, scytherVariety.UniqueName, new FormTypes(PokemonType.Bug, PokemonType.Flying),
      new FormAbilities(swarm), new BaseStatistics(70, 110, 80, 55, 80, 105), new Yield(100, 0, 1, 0, 0, 0, 0),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/scyther.png"), new Url("https://www.pokegame.com/assets/img/pokemon/scyther-shiny.png")), isDefault: true);
    Form scizorForm = new(scizorVariety, scizorVariety.UniqueName, new FormTypes(PokemonType.Bug, PokemonType.Steel),
      new FormAbilities(swarm), new BaseStatistics(70, 130, 100, 55, 80, 65), new Yield(175, 0, 2, 0, 0, 0, 0),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/scizor.png"), new Url("https://www.pokegame.com/assets/img/pokemon/scizor-shiny.png")), isDefault: true);

    Item metalCoat = new(new UniqueName(_uniqueNameSettings, "metal-coat"), new OtherItemProperties());
    Evolution evolution = new(scytherForm, scizorForm, EvolutionTrigger.Trade)
    {
      HeldItemId = metalCoat.Id
    };

    Specimen pokemon = new(scytherSpecies, scytherVariety, scytherForm, scytherSpecies.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), _randomizer.PokemonGender(scytherVariety.GenderRatio!));
    pokemon.HoldItem(metalCoat);
    PokemonStatistics statistics = pokemon.Statistics;

    Item pokeBall = new(new UniqueName(_uniqueNameSettings, "poke-ball"), new PokeBallProperties());
    Location location = new("La Forest");

    Trainer original = new(new License("Q-123456-3"), new UniqueName(_uniqueNameSettings, "regina"), TrainerGender.Female);
    pokemon.Catch(original, pokeBall, location);

    Trainer current = new(new License("Q-590921-8"), new UniqueName(_uniqueNameSettings, "jean-guy-bowlpacker"), TrainerGender.Male);
    pokemon.Receive(current, pokeBall, location);

    pokemon.Evolve(scizorSpecies, scizorVariety, scizorForm, evolution, _actorId);
    Assert.Equal(scizorSpecies.Id, pokemon.SpeciesId);
    Assert.Equal(scizorVariety.Id, pokemon.VarietyId);
    Assert.Equal(scizorForm.Id, pokemon.FormId);
    Assert.Equal(scizorForm.BaseStatistics, pokemon.BaseStatistics);
    Assert.NotEqual(statistics, pokemon.Statistics);
    Assert.True(pokemon.HasChanges);
    Assert.Contains(pokemon.Changes, change => change is PokemonEvolved evolved && evolved.ActorId == _actorId);
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the evolution trigger is Trade, but the Pokémon has not been traded.")]
  public void Given_TradeNotTraded_When_Evolve_Then_ValidationException()
  {
    Evolution evolution = new(_rowletForm, _dartrixForm, EvolutionTrigger.Trade);
    var exception = Assert.Throws<ValidationException>(() => _rowlet.Evolve(_dartrixSpecies, _dartrixVariety, _dartrixForm, evolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "PokemonId" && e.AttemptedValue?.Equals(_rowlet.Id.ToGuid()) == true
      && e.ErrorCode == "PokemonNotTraded" && e.ErrorMessage == "The Pokémon current and original trainers must be different for traded evolutions.");
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the form does not belong to the variety.")]
  public void Given_InvalidForm_When_Evolve_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => _rowlet.Evolve(_dartrixSpecies, _dartrixVariety, _rowletForm, _rowletEvolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "form" && e.AttemptedValue?.Equals(_rowletForm.Id) == true
      && e.ErrorCode == "InvalidForm" && e.ErrorMessage == "The form does not belong to the variety." && e.CustomState is not null);
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the form is not the evolution target.")]
  public void Given_FormNotTarget_When_Evolve_Then_ValidationException()
  {
    Form form = new(_dartrixVariety, new UniqueName(_uniqueNameSettings, "alt-dartrix"), _dartrixForm.Types, _dartrixForm.Abilities,
      _dartrixForm.BaseStatistics, _dartrixForm.Yield, _dartrixForm.Sprites, height: _dartrixForm.Height, weight: _dartrixForm.Weight);

    var exception = Assert.Throws<ValidationException>(() => _rowlet.Evolve(_dartrixSpecies, _dartrixVariety, form, _rowletEvolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "form" && e.AttemptedValue?.Equals(form.Id) == true
      && e.ErrorCode == "InvalidTargetForm" && e.ErrorMessage == "The form is not the target evolution form." && e.CustomState is not null);
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the Pokémon form is not the evolution source.")]
  public void Given_PokemonFormNotSource_When_Evolve_Then_ValidationException()
  {
    Evolution evolution = new(_dartrixForm, _rowletForm);
    var exception = Assert.Throws<ValidationException>(() => _rowlet.Evolve(_dartrixSpecies, _dartrixVariety, _dartrixForm, evolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "EvolutionId" && e.AttemptedValue?.Equals(evolution.Id.ToGuid()) == true
      && e.ErrorCode == "InvalidSourceForm" && e.ErrorMessage == "The Pokémon form should be the evolution source form." && e.CustomState is not null);
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the Pokémon gender is not the requirement.")]
  public void Given_GenderRequirementNotMet_When_Evolve_Then_ValidationException()
  {
    PokemonSpecies combeeSpecies = new(new Number(415), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "combee"));
    PokemonSpecies vespiquenSpecies = new(new Number(416), PokemonCategory.Standard, new UniqueName(_uniqueNameSettings, "vespiquen"));

    Variety combeeVariety = new(combeeSpecies, combeeSpecies.UniqueName, isDefault: true, new GenderRatio(7));
    Variety vespiquenVariety = new(vespiquenSpecies, vespiquenSpecies.UniqueName, isDefault: true, new GenderRatio(0));

    Ability honeyGather = new(new UniqueName(_uniqueNameSettings, "honey-gather"));
    Form combeeForm = new(combeeVariety, combeeVariety.UniqueName, new FormTypes(PokemonType.Bug, PokemonType.Flying),
      new FormAbilities(honeyGather), new BaseStatistics(30, 30, 42, 30, 42, 70), new Yield(49, 0, 0, 0, 0, 0, 1),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/combee.png"), new Url("https://www.pokegame.com/assets/img/pokemon/combee-shiny.png")));
    Form vespiquenForm = new(vespiquenVariety, vespiquenVariety.UniqueName, new FormTypes(PokemonType.Bug, PokemonType.Flying),
      new FormAbilities(honeyGather), new BaseStatistics(70, 80, 102, 80, 102, 40), new Yield(166, 0, 0, 1, 0, 1, 0),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/vespiquen.png"), new Url("https://www.pokegame.com/assets/img/pokemon/vespiquen-shiny.png")));

    Evolution evolution = new(combeeForm, vespiquenForm)
    {
      Level = new Level(21),
      Gender = PokemonGender.Female
    };

    Specimen combee = new(combeeSpecies, combeeVariety, combeeForm, combeeSpecies.UniqueName, _randomizer.PokemonSize(),
      _randomizer.PokemonNature(), _randomizer.IndividualValues(), PokemonGender.Male, experience: 11735);
    var exception = Assert.Throws<ValidationException>(() => combee.Evolve(vespiquenSpecies, vespiquenVariety, vespiquenForm, evolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "Gender" && e.AttemptedValue?.Equals(combee.Gender) == true
      && e.ErrorCode == "GenderRequirementNotMet" && e.ErrorMessage == "The Pokémon gender must be 'Female'.");
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the Pokémon friendship is lower than the requirement.")]
  public void Given_FriendshipRequirementNotMet_When_Evolve_Then_ValidationException()
  {
    Evolution evolution = new(_rowletForm, _dartrixForm)
    {
      Friendship = true
    };

    var exception = Assert.Throws<ValidationException>(() => _rowlet.Evolve(_dartrixSpecies, _dartrixVariety, _dartrixForm, evolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "Friendship" && e.AttemptedValue?.Equals(_rowlet.Friendship.Value) == true
      && e.ErrorCode == "FriendshipRequirementNotMet" && e.ErrorMessage == "The Pokémon friendship must be greater than or equal to 200.");
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the Pokémon held item is not the requirement.")]
  public void Given_HeldItemRequirementNotMet_When_Evolve_Then_ValidationException()
  {
    Evolution evolution = new(_rowletForm, _dartrixForm)
    {
      HeldItemId = ItemId.NewId()
    };

    var exception = Assert.Throws<ValidationException>(() => _rowlet.Evolve(_dartrixSpecies, _dartrixVariety, _dartrixForm, evolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "HeldItemId" && e.AttemptedValue is null
      && e.ErrorCode == "HeldItemRequirementNotMet" && e.ErrorMessage == $"The Pokémon must hold the item 'Id={evolution.HeldItemId.Value.ToGuid()}'.");
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the Pokémon known move requirement is not met.")]
  public void Given_KnownMoveRequirementNotMet_When_Evolve_Then_ValidationException()
  {
    Evolution evolution = new(_rowletForm, _dartrixForm)
    {
      KnownMoveId = MoveId.NewId()
    };

    var exception = Assert.Throws<ValidationException>(() => _rowlet.Evolve(_dartrixSpecies, _dartrixVariety, _dartrixForm, evolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "CurrentMoves" && e.AttemptedValue is IEnumerable<Guid>
      && e.ErrorCode == "KnownMoveRequirementNotMet" && e.ErrorMessage == $"The Pokémon must currently know the move 'Id={evolution.KnownMoveId.Value.ToGuid()}'.");
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the Pokémon level is lower than the requirement.")]
  public void Given_LevelRequirementNotMet_When_Evolve_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => _rowlet.Evolve(_dartrixSpecies, _dartrixVariety, _dartrixForm, _rowletEvolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "Level" && e.AttemptedValue?.Equals(_rowlet.Level) == true
      && e.ErrorCode == "LevelRequirementNotMet" && e.ErrorMessage == "The Pokémon level must be greater than or equal to 17.");
  }

  [Fact(DisplayName = "Evolve: it should throw ValidationException when the variety does not belong to the species.")]
  public void Given_InvalidVariety_When_Evolve_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => _rowlet.Evolve(_dartrixSpecies, _rowletVariety, _rowletForm, _rowletEvolution));
    Assert.Contains(exception.Errors, e => e.PropertyName == "variety" && e.AttemptedValue?.Equals(_rowletVariety.Id) == true
      && e.ErrorCode == "InvalidVariety" && e.ErrorMessage == "The variety does not belong to the species." && e.CustomState is not null);
  }
}
