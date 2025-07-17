using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonDeleteTests
{
  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Pokemon2 _pokemon;

  public PokemonDeleteTests()
  {
    UniqueNameSettings uniqueNameSettings = new();

    _species = new PokemonSpecies(new Number(499), PokemonCategory.Standard, new UniqueName(uniqueNameSettings, "pignite"), new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow);

    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(7));

    Ability blaze = new(new UniqueName(uniqueNameSettings, "blaze"));
    Ability thickFat = new(new UniqueName(uniqueNameSettings, "thick-fat"));
    _form = new Form(_variety, _variety.UniqueName, new Types(PokemonType.Fire, PokemonType.Fighting),
      new FormAbilities(blaze, secondary: null, thickFat), new BaseStatistics(90, 93, 55, 70, 55, 55), isDefault: true);

    _pokemon = new Pokemon2(
      _species,
      _variety,
      _form,
      new UniqueName(uniqueNameSettings, "briquet"),
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Find("careful"),
      new IndividualValues(),
      PokemonGender.Male);
  }

  [Fact(DisplayName = "It should delete the Pokémon.")]
  public void Given_Pokemon_When_Delete_Then_IsDeleted()
  {
    ActorId actorId = ActorId.NewId();
    Assert.False(_pokemon.IsDeleted);

    _pokemon.Delete(actorId);
    Assert.True(_pokemon.IsDeleted);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonDeleted deleted && deleted.ActorId == actorId);

    _pokemon.ClearChanges();
    _pokemon.Delete();
    Assert.True(_pokemon.IsDeleted);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }
}
