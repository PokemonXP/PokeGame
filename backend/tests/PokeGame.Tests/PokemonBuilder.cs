using Bogus;
using Krakenar.Core;
using Krakenar.Core.Settings;
using PokeGame.Core;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame;

public class PokemonBuilder
{
  private readonly Faker _faker;
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;

  private readonly Dictionary<SpeciesId, PokemonSpecies> _species = [];
  private readonly Dictionary<VarietyId, Variety> _varieties = [];
  private readonly Dictionary<FormId, Form> _forms = [];

  private EggCycles? _eggCycles = null;
  private PokemonSpecies? _selectedSpecies = null;

  public PokemonBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();

    UniqueNameSettings uniqueNameSettings = new();

    Ability adaptability = new(new UniqueName(uniqueNameSettings, "adaptability"));
    Ability anticipation = new(new UniqueName(uniqueNameSettings, "anticipation"));
    Ability honeyGather = new(new UniqueName(uniqueNameSettings, "honey-gather"));
    Ability longReach = new(new UniqueName(uniqueNameSettings, "long-reach"));
    Ability overgrow = new(new UniqueName(uniqueNameSettings, "overgrow"));
    Ability runAway = new(new UniqueName(uniqueNameSettings, "run-away"));
    Ability shieldDust = new(new UniqueName(uniqueNameSettings, "shield-dust"));
    Ability sweetVeil = new(new UniqueName(uniqueNameSettings, "sweet-veil"));

    PokemonSpecies eeveeSpecies = new(new Number(133), PokemonCategory.Standard, new UniqueName(uniqueNameSettings, "eevee"),
      new Friendship(70), new CatchRate(45), GrowthRate.MediumFast, new EggCycles(35), new EggGroups(EggGroup.Field));
    _species[eeveeSpecies.Id] = eeveeSpecies;
    PokemonSpecies rowletSpecies = new(new Number(722), PokemonCategory.Standard, new UniqueName(uniqueNameSettings, "rowlet"),
      new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow, new EggCycles(15), new EggGroups(EggGroup.Flying));
    _species[rowletSpecies.Id] = rowletSpecies;
    PokemonSpecies cutieflySpecies = new(new Number(742), PokemonCategory.Standard, new UniqueName(uniqueNameSettings, "cutiefly"),
      new Friendship(70), new CatchRate(190), GrowthRate.MediumFast, new EggCycles(20), new EggGroups(EggGroup.Bug, EggGroup.Fairy));
    _species[cutieflySpecies.Id] = cutieflySpecies;

    Variety eeveeVariety = new(eeveeSpecies, eeveeSpecies.UniqueName, isDefault: true, new GenderRatio(7));
    _varieties[eeveeVariety.Id] = eeveeVariety;
    Variety rowletVariety = new(rowletSpecies, rowletSpecies.UniqueName, isDefault: true, new GenderRatio(7));
    _varieties[rowletVariety.Id] = rowletVariety;
    Variety cutieflyVariety = new(cutieflySpecies, cutieflySpecies.UniqueName, isDefault: true, new GenderRatio(4));
    _varieties[cutieflyVariety.Id] = cutieflyVariety;

    Form eeveeForm = new(eeveeVariety, eeveeVariety.UniqueName, new FormTypes(PokemonType.Normal),
      new FormAbilities(runAway, adaptability, anticipation), new BaseStatistics(68, 55, 55, 50, 50, 42), new Yield(65, 0, 0, 0, 0, 1, 0),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/eevee.png"), new Url("https://www.pokegame.com/assets/img/pokemon/eevee-shiny.png")),
      isDefault: true, height: new Height(3), weight: new Weight(65));
    _forms[eeveeForm.Id] = eeveeForm;
    Form rowletForm = new(rowletVariety, rowletVariety.UniqueName, new FormTypes(PokemonType.Grass, PokemonType.Flying),
      new FormAbilities(overgrow, secondary: null, longReach), new BaseStatistics(68, 55, 55, 50, 50, 42), new Yield(64, 1, 0, 0, 0, 0, 0),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/rowlet.png"), new Url("https://www.pokegame.com/assets/img/pokemon/rowlet-shiny.png")),
      isDefault: true, height: new Height(3), weight: new Weight(15));
    _forms[rowletForm.Id] = rowletForm;
    Form cutieflyForm = new(cutieflyVariety, cutieflyVariety.UniqueName, new FormTypes(PokemonType.Bug, PokemonType.Fairy),
      new FormAbilities(honeyGather, shieldDust, sweetVeil), new BaseStatistics(40, 45, 40, 55, 40, 84), new Yield(61, 0, 0, 0, 0, 0, 1),
      new Sprites(new Url("https://www.pokegame.com/assets/img/pokemon/cutiefly.png"), new Url("https://www.pokegame.com/assets/img/pokemon/cutiefly-shiny.png")),
      isDefault: true, height: new Height(1), weight: new Weight(2));
    _forms[cutieflyForm.Id] = cutieflyForm;
  }

  public PokemonBuilder IsEgg(byte? remainingCycles = null)
  {
    if (remainingCycles.HasValue)
    {
      _eggCycles = new EggCycles(remainingCycles.Value);
    }
    else
    {
      _eggCycles = _selectedSpecies?.EggCycles ?? throw new InvalidOperationException("A species must have been selected.");
    }

    return this;
  }

  public PokemonBuilder WithSpecies(string uniqueName)
  {
    PokemonSpecies[] species = _species.Values
      .Where(x => x.UniqueName.Value.Trim().Equals(uniqueName.Trim(), StringComparison.InvariantCultureIgnoreCase))
      .ToArray();
    if (species.Length < 1)
    {
      throw new ArgumentException($"The Pokémon species 'UniqueName={uniqueName}' was not found.", nameof(uniqueName));
    }
    else if (species.Length > 1)
    {
      throw new ArgumentException($"Multiple Pokémon species were found for unique name '{uniqueName}'.", nameof(uniqueName));
    }
    _selectedSpecies = species.Single();
    return this;
  }

  public Specimen Build()
  {
    Form form = _faker.PickRandom(_forms.Values.ToArray());
    Variety variety = _varieties[form.VarietyId];
    PokemonSpecies species = _species[variety.SpeciesId];

    if (_selectedSpecies is not null)
    {
      species = _selectedSpecies;
      variety = _varieties.Values.Single(x => x.SpeciesId == species.Id && x.IsDefault);
      form = _forms.Values.Single(x => x.VarietyId == variety.Id && x.IsDefault);
    }

    return new Specimen(
      species,
      variety,
      form,
      species.UniqueName,
      _randomizer.PokemonSize(),
      _randomizer.PokemonNature(),
      _randomizer.IndividualValues(),
      variety.GenderRatio is null ? null : _randomizer.PokemonGender(variety.GenderRatio),
      eggCycles: _eggCycles);
  }
}
