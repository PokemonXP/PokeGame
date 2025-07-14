using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonTests
{
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly Pokemon _briquet;
  private readonly Pokemon _hedwidge;

  public PokemonTests()
  {
    _briquet = new Pokemon(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Careful,
      new BaseStatistics(90, 93, 55, 70, 55, 55));
    _hedwidge = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "rowlet"),
      PokemonType.Grass,
      _randomizer.PokemonSize(),
      PokemonNatures.Instance.Gentle,
      new BaseStatistics(60, 55, 55, 50, 50, 42),
      PokemonGender.Male,
      AbilitySlot.Primary,
      new IndividualValues(24, 31, 21, 25, 22, 23),
      new EffortValues(),
      GrowthRate.MediumSlow,
      experience: 135,
      vitality: int.MaxValue,
      stamina: int.MaxValue,
      friendship: 70);
  }

  [Fact(DisplayName = "It should assign the correct characteristic.")]
  public void Given_IVsAndSize_When_ctor_Then_CorrectCharacteristic()
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
      new IndividualValues(27, 27, 25, 22, 25, 26));
    Assert.Equal("Nods off a lot", pokemon.Characteristic.Text);
  }

  [Fact(DisplayName = "It should calculate the correct experience to next level.")]
  public void Given_Experience_When_ToNextLevel_Then_Correct()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
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
      _randomizer.PokemonSize(),
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
      _randomizer.PokemonSize(),
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

  [Fact(DisplayName = "It should handle Gender changes correctly.")]
  public void Given_Pokemon_When_Gender_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    _hedwidge.Gender = PokemonGender.Female;
    _hedwidge.Update(actorId);
    Assert.Equal(PokemonGender.Female, _hedwidge.Gender);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId
      && updated.Gender == new Change<PokemonGender?>(PokemonGender.Female));

    _hedwidge.ClearChanges();
    _hedwidge.Gender = PokemonGender.Female;
    Assert.Equal(PokemonGender.Female, _hedwidge.Gender);
    _hedwidge.Update(actorId);
    Assert.False(_hedwidge.HasChanges);

    _hedwidge.Gender = null;
    _hedwidge.Update(actorId);
    Assert.Null(_hedwidge.Gender);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId
      && updated.Gender == new Change<PokemonGender?>(null));
  }

  [Fact(DisplayName = "It should handle Notes changes correctly.")]
  public void Given_Pokemon_When_Notes_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    Description notes = new("This is the starter Pokémon of Jean-Guy Bowlpacker.");
    _hedwidge.Notes = notes;
    Assert.Equal(notes, _hedwidge.Notes);
    _hedwidge.Update(actorId);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Notes == new Change<Description>(notes));

    _hedwidge.ClearChanges();
    _hedwidge.Notes = notes;
    Assert.Equal(notes, _hedwidge.Notes);
    _hedwidge.Update(actorId);
    Assert.False(_hedwidge.HasChanges);

    _hedwidge.Notes = null;
    _hedwidge.Update(actorId);
    Assert.Null(_hedwidge.Notes);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Notes == new Change<Description>(null));
  }

  [Fact(DisplayName = "It should handle Sprite changes correctly.")]
  public void Given_Pokemon_When_Sprite_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    Url sprite = new("https://pokegame.blob.core.windows.net/pokemon/rowlet.png");
    _hedwidge.Sprite = sprite;
    Assert.Equal(sprite, _hedwidge.Sprite);
    _hedwidge.Update(actorId);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Sprite == new Change<Url>(sprite));

    _hedwidge.ClearChanges();
    _hedwidge.Sprite = sprite;
    Assert.Equal(sprite, _hedwidge.Sprite);
    _hedwidge.Update(actorId);
    Assert.False(_hedwidge.HasChanges);

    _hedwidge.Sprite = null;
    _hedwidge.Update(actorId);
    Assert.Null(_hedwidge.Sprite);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Sprite == new Change<Url>(null));
  }

  [Fact(DisplayName = "It should handle Url changes correctly.")]
  public void Given_Pokemon_When_Url_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    Url url = new("https://docs.google.com/spreadsheets/d/1_QczIHx3bQBXJM4jjYl_p9t8lNrTMZDuclr9V11bhUI/edit?usp=sharing");
    _hedwidge.Url = url;
    Assert.Equal(url, _hedwidge.Url);
    _hedwidge.Update(actorId);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Url == new Change<Url>(url));

    _hedwidge.ClearChanges();
    _hedwidge.Url = url;
    Assert.Equal(url, _hedwidge.Url);
    _hedwidge.Update(actorId);
    Assert.False(_hedwidge.HasChanges);

    _hedwidge.Url = null;
    _hedwidge.Update(actorId);
    Assert.Null(_hedwidge.Url);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonUpdated updated && updated.ActorId == actorId && updated.Url == new Change<Url>(null));
  }

  [Fact(DisplayName = "It should not create a Pokémon with more Stamina than its maximum.")]
  public void Given_StaminaGreaterThanMaximum_When_ctor_Then_MaximumStamina()
  {
    Pokemon pokemon = new(
      FormId.NewId(),
      new UniqueName(new UniqueNameSettings(), "elliotto-briquet"),
      PokemonType.Fire,
      _randomizer.PokemonSize(),
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
      _randomizer.PokemonSize(),
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
    Assert.Equal(Flavor.Bitter, _briquet.FavoriteFlavor);
    Assert.Equal(Flavor.Dry, _briquet.DislikedFlavor);
  }

  [Fact(DisplayName = "SetNickname: it should handle nickname changes correctly.")]
  public void Given_Nickname_When_SetNickname_Then_NicknameChanged()
  {
    ActorId actorId = ActorId.NewId();

    Assert.Null(_hedwidge.Nickname);

    DisplayName nickname = new("Hedwidge");
    _hedwidge.SetNickname(nickname, actorId);
    Assert.Equal(nickname, _hedwidge.Nickname);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonNicknamed nicknamed && nicknamed.ActorId == actorId && nicknamed.Nickname == nickname);
    Assert.StartsWith(nickname.Value, _hedwidge.ToString());

    _hedwidge.ClearChanges();
    _hedwidge.SetNickname(nickname, actorId);
    Assert.False(_hedwidge.HasChanges);

    _hedwidge.SetNickname(nickname: null, actorId);
    Assert.Null(_hedwidge.Nickname);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonNicknamed nicknamed && nicknamed.ActorId == actorId && nicknamed.Nickname is null);
  }

  [Fact(DisplayName = "SetUniqueName: it should handle unique name changes correctly.")]
  public void Given_UniqueName_When_SetUniqueName_Then_UniqueNameChanged()
  {
    ActorId actorId = ActorId.NewId();

    UniqueName uniqueName = new(new UniqueNameSettings(), "bowlpacker-rowlet");
    _hedwidge.SetUniqueName(uniqueName, actorId);
    Assert.Equal(uniqueName, _hedwidge.UniqueName);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonUniqueNameChanged changed && changed.ActorId == actorId && changed.UniqueName == uniqueName);
    Assert.StartsWith(uniqueName.Value, _hedwidge.ToString());

    _hedwidge.ClearChanges();
    _hedwidge.SetUniqueName(uniqueName, actorId);
    Assert.False(_hedwidge.HasChanges);
  }

  // TODO(fpion): Vitality
  // TODO(fpion): Stamina
  // TODO(fpion): StatusCondition
  // TODO(fpion): Friendship
}
