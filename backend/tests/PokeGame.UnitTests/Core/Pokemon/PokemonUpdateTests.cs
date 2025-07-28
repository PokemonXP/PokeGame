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
public class PokemonUpdateTests
{
  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Specimen _pokemon;

  public PokemonUpdateTests()
  {
    UniqueNameSettings uniqueNameSettings = new();

    _species = new PokemonSpecies(new Number(499), PokemonCategory.Standard, new UniqueName(uniqueNameSettings, "pignite"), new Friendship(70), new CatchRate(45), GrowthRate.MediumSlow);

    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(7));

    Ability blaze = new(new UniqueName(uniqueNameSettings, "blaze"));
    Ability thickFat = new(new UniqueName(uniqueNameSettings, "thick-fat"));
    Sprites sprites = new(new Url("https://www.pokegame.com/assets/img/pokemon/pignite.png"), new Url("https://www.pokegame.com/assets/img/pokemon/pignite-shiny.png"));
    _form = new Form(_variety, _variety.UniqueName, new FormTypes(PokemonType.Fire, PokemonType.Fighting),
      new FormAbilities(blaze, secondary: null, thickFat), new BaseStatistics(90, 93, 55, 70, 55, 55),
      new Yield(146, 0, 2, 0, 0, 0, 0), sprites, isDefault: true, height: new Height(10), weight: new Weight(555));

    _pokemon = new Specimen(
      _species,
      _variety,
      _form,
      new UniqueName(uniqueNameSettings, "briquet"),
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Find("careful"),
      new IndividualValues(),
      PokemonGender.Male);
  }

  [Fact(DisplayName = "It should handle Friendship changes correctly.")]
  public void Given_Pokemon_When_Friendship_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    Friendship friendship = new(200);
    _pokemon.Friendship = friendship;
    Assert.Equal(friendship, _pokemon.Friendship);
    _pokemon.Update(actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Friendship == friendship);

    _pokemon.ClearChanges();
    _pokemon.Friendship = friendship;
    Assert.Equal(friendship, _pokemon.Friendship);
    _pokemon.Update(actorId);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "It should handle Notes changes correctly.")]
  public void Given_Pokemon_When_Notes_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    Notes notes = new("This is the starter Pokémon of Elliotto.");
    _pokemon.Notes = notes;
    Assert.Equal(notes, _pokemon.Notes);
    _pokemon.Update(actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Notes == new Change<Notes>(notes));

    _pokemon.ClearChanges();
    _pokemon.Notes = notes;
    Assert.Equal(notes, _pokemon.Notes);
    _pokemon.Update(actorId);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);

    _pokemon.Notes = null;
    Assert.Null(_pokemon.Notes);
    _pokemon.Update(actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Notes == new Change<Notes>(null));
  }

  [Fact(DisplayName = "It should handle Shinyness changes correctly.")]
  public void Given_Pokemon_When_Shinyness_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    bool isShiny = !_pokemon.IsShiny;
    _pokemon.IsShiny = isShiny;
    Assert.Equal(isShiny, _pokemon.IsShiny);
    _pokemon.Update(actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.IsShiny == isShiny);

    _pokemon.ClearChanges();
    _pokemon.IsShiny = isShiny;
    Assert.Equal(isShiny, _pokemon.IsShiny);
    _pokemon.Update(actorId);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "It should handle Sprite changes correctly.")]
  public void Given_Pokemon_When_Sprite_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    Url sprite = new("https://pokegame.blob.core.windows.net/pokemon/tepig.png");
    _pokemon.Sprite = sprite;
    Assert.Equal(sprite, _pokemon.Sprite);
    _pokemon.Update(actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Sprite == new Change<Url>(sprite));

    _pokemon.ClearChanges();
    _pokemon.Sprite = sprite;
    Assert.Equal(sprite, _pokemon.Sprite);
    _pokemon.Update(actorId);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);

    _pokemon.Sprite = null;
    Assert.Null(_pokemon.Sprite);
    _pokemon.Update(actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Sprite == new Change<Url>(null));
  }

  [Fact(DisplayName = "It should handle Stamina changes correctly.")]
  public void Given_Pokemon_When_Stamina_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    int stamina = _pokemon.Stamina / 2;
    _pokemon.Stamina = stamina;
    Assert.Equal(stamina, _pokemon.Stamina);
    _pokemon.Update(actorId);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Stamina == stamina);

    _pokemon.ClearChanges();
    _pokemon.Stamina = stamina;
    Assert.Equal(stamina, _pokemon.Stamina);
    _pokemon.Update(actorId);
    Assert.False(_pokemon.HasChanges);

    _pokemon.Stamina = int.MaxValue;
    _pokemon.Update(actorId);
    int constitution = _pokemon.Statistics.HP;
    Assert.Equal(constitution, _pokemon.Stamina);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Stamina == constitution);
  }

  [Fact(DisplayName = "It should handle Status Condition changes correctly.")]
  public void Given_Pokemon_When_StatusCondition_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    StatusCondition statusCondition = StatusCondition.Sleep;
    _pokemon.StatusCondition = statusCondition;
    Assert.Equal(statusCondition, _pokemon.StatusCondition);
    _pokemon.Update(actorId);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.StatusCondition == new Change<StatusCondition?>(statusCondition));

    _pokemon.ClearChanges();
    _pokemon.StatusCondition = statusCondition;
    Assert.Equal(statusCondition, _pokemon.StatusCondition);
    _pokemon.Update(actorId);
    Assert.False(_pokemon.HasChanges);

    _pokemon.StatusCondition = null;
    Assert.Null(_pokemon.StatusCondition);
    Assert.Null(_pokemon.StatusCondition); _pokemon.Update(actorId);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.StatusCondition == new Change<StatusCondition?>(null));
  }

  [Fact(DisplayName = "It should handle Url changes correctly.")]
  public void Given_Pokemon_When_Url_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    Url url = new("https://docs.google.com/spreadsheets/d/1n9TygZwRcbcLU8o60QaZ4md3QwcXNoG9pEFdvPkDY3o/edit?usp=sharing");
    _pokemon.Url = url;
    Assert.Equal(url, _pokemon.Url);
    _pokemon.Update(actorId);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Url == new Change<Url>(url));

    _pokemon.ClearChanges();
    _pokemon.Url = url;
    Assert.Equal(url, _pokemon.Url);
    _pokemon.Update(actorId);
    Assert.False(_pokemon.HasChanges);

    _pokemon.Url = null;
    Assert.Null(_pokemon.Url);
    _pokemon.Update(actorId);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Url == new Change<Url>(null));
  }

  [Fact(DisplayName = "It should handle Vitality changes correctly.")]
  public void Given_Pokemon_When_Vitality_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    int vitality = _pokemon.Vitality / 2;
    _pokemon.Vitality = vitality;
    Assert.Equal(vitality, _pokemon.Vitality);
    _pokemon.Update(actorId);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Vitality == vitality);

    _pokemon.ClearChanges();
    _pokemon.Vitality = vitality;
    Assert.Equal(vitality, _pokemon.Vitality);
    _pokemon.Update(actorId);
    Assert.False(_pokemon.HasChanges);

    _pokemon.Vitality = int.MaxValue;
    _pokemon.Update(actorId);
    int constitution = _pokemon.Statistics.HP;
    Assert.Equal(constitution, _pokemon.Vitality);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Vitality == constitution);
  }
}
