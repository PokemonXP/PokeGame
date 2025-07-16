using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Speciez;
using PokeGame.Core.Varieties;
using PokemonSpecies = PokeGame.Core.Speciez.Species;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonNameTests
{
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly PokemonSpecies _species;
  private readonly Variety _variety;
  private readonly Form _form;
  private readonly Pokemon2 _pokemon;

  public PokemonNameTests()
  {
    _species = new PokemonSpecies(number: 499, new UniqueName(_uniqueNameSettings, "pignite"),
      new CatchRate(45), PokemonCategory.Standard, new Friendship(70), GrowthRate.MediumSlow);

    _variety = new Variety(_species, _species.UniqueName, isDefault: true, new GenderRatio(7));

    Ability blaze = new(new UniqueName(_uniqueNameSettings, "blaze"));
    Ability thickFat = new(new UniqueName(_uniqueNameSettings, "thick-fat"));
    _form = new Form(_variety, _variety.UniqueName, new Types(PokemonType.Fire, PokemonType.Fighting),
      new FormAbilities(blaze, secondary: null, thickFat), new BaseStatistics(90, 93, 55, 70, 55, 55), isDefault: true);

    _pokemon = new Pokemon2(
      _species,
      _variety,
      _form,
      new UniqueName(_uniqueNameSettings, "briquet"),
      new PokemonSize(128, 128),
      PokemonNatures.Instance.Find("careful"),
      new IndividualValues(),
      PokemonGender.Male);
  }

  [Fact(DisplayName = "SetNickname: it should handle nickname changes correctly.")]
  public void Given_Nickname_When_SetNickname_Then_NicknameChanged()
  {
    ActorId actorId = ActorId.NewId();
    Assert.Null(_pokemon.Nickname);

    Nickname nickname = new("Briquet");
    _pokemon.SetNickname(nickname, actorId);
    Assert.Equal(nickname, _pokemon.Nickname);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonNicknamed2 nicknamed && nicknamed.ActorId == actorId && nicknamed.Nickname == nickname);

    _pokemon.ClearChanges();
    _pokemon.SetNickname(nickname);
    Assert.Equal(nickname, _pokemon.Nickname);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);

    _pokemon.SetNickname(nickname: null, actorId);
    Assert.Null(_pokemon.Nickname);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonNicknamed2 nicknamed && nicknamed.ActorId == actorId && nicknamed.Nickname is null);
  }

  [Fact(DisplayName = "SetUniqueName: it should handle unique name changes correctly.")]
  public void Given_UniqueName_When_SetUniqueName_Then_UniqueNameChanged()
  {
    ActorId actorId = ActorId.NewId();
    Assert.Null(_pokemon.Nickname);

    UniqueName uniqueName = _species.UniqueName;
    _pokemon.SetUniqueName(uniqueName, actorId);
    Assert.Equal(uniqueName, _pokemon.UniqueName);
    Assert.True(_pokemon.HasChanges);
    Assert.Contains(_pokemon.Changes, change => change is PokemonUniqueNameChanged changed && changed.ActorId == actorId && changed.UniqueName == uniqueName);

    _pokemon.ClearChanges();
    _pokemon.SetUniqueName(uniqueName);
    Assert.Equal(uniqueName, _pokemon.UniqueName);
    Assert.False(_pokemon.HasChanges);
    Assert.Empty(_pokemon.Changes);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Pokemon_When_ToString_Then_CorrectString()
  {
    Assert.StartsWith(_pokemon.UniqueName.Value, _pokemon.ToString());

    Nickname nickname = new("Briquet");
    _pokemon.SetNickname(nickname);
    Assert.StartsWith(nickname.Value, _pokemon.ToString());
  }
}
