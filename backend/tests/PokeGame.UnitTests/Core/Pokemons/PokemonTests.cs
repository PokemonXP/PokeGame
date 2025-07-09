using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;

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

  [Fact(DisplayName = "HoldItem: it should handle held item changes correctly.")]
  public void Given_HeldItem_When_Change_Then_CorrectChanges()
  {
    Assert.Null(_briquet.HeldItemId);
    _briquet.ClearChanges();

    _briquet.RemoveItem();
    Assert.Null(_briquet.HeldItemId);
    Assert.False(_briquet.HasChanges);

    ItemId itemId = ItemId.NewId();
    ActorId actorId = ActorId.NewId();
    _briquet.HoldItem(itemId, actorId);
    Assert.Equal(itemId, _briquet.HeldItemId);
    Assert.Contains(_briquet.Changes, change => change is PokemonItemHeld held && held.ItemId == itemId && held.ActorId == actorId);

    _briquet.ClearChanges();
    _briquet.HoldItem(itemId, actorId);
    Assert.Equal(itemId, _briquet.HeldItemId);
    Assert.False(_briquet.HasChanges);

    _briquet.RemoveItem(actorId);
    Assert.Null(_briquet.HeldItemId);
    Assert.Contains(_briquet.Changes, change => change is PokemonItemRemoved removed && removed.ActorId == actorId);
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

  [Theory(DisplayName = "LearnMove: it should add the move to the list when the Pokémon has not reached the move limit.")]
  [InlineData(null)]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void Given_MoveLimitNotReached_When_LearnMove_Then_MoveAdded(int? position)
  {
    Pokemon pokemon = new(
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
      experience: 179,
      vitality: int.MaxValue,
      stamina: int.MaxValue,
      friendship: 70);
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(35)); // NOTE(fpion): Tackle
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Growl
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Leafage

    MoveId moveId = MoveId.NewId(); // NOTE(fpion): Astonish
    PowerPoints powerPoints = new(15);
    ActorId actorId = ActorId.NewId();
    Assert.True(pokemon.LearnMove(moveId, powerPoints, position, actorId));

    Assert.Contains(pokemon.Changes, change => change is PokemonMoveLearned learned && learned.ActorId == actorId
      && learned.PowerPoints == powerPoints && learned.Position == null && !learned.TechnicalMachine);

    PokemonMove move = new(moveId, powerPoints.Value, powerPoints.Value, powerPoints, IsMastered: false, pokemon.Level, TechnicalMachine: false);
    Assert.Equal(move, pokemon.AllMoves[move.MoveId]);
    Assert.Equal(Pokemon.MoveLimit, pokemon.Moves.Count);
    Assert.Equal(move, pokemon.Moves.Last());
  }

  [Theory(DisplayName = "LearnMove: it should replace a move in the list when the Pokémon has reached the move limit.")]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void Given_MoveLimitReached_When_LearnMove_Then_MoveReplaced(int position)
  {
    MoveId tackle = MoveId.NewId();
    MoveId growl = MoveId.NewId();
    MoveId leafage = MoveId.NewId();
    MoveId astonish = MoveId.NewId();
    MoveId peck = MoveId.NewId();

    Pokemon pokemon = new(
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
      experience: 419,
      vitality: int.MaxValue,
      stamina: int.MaxValue,
      friendship: 70);
    pokemon.LearnMove(tackle, new PowerPoints(35));
    pokemon.LearnMove(growl, new PowerPoints(40));
    pokemon.LearnMove(leafage, new PowerPoints(40));
    pokemon.LearnMove(astonish, new PowerPoints(15));

    PowerPoints powerPoints = new(40);
    ActorId actorId = ActorId.NewId();
    Assert.True(pokemon.LearnMove(peck, powerPoints, position, actorId));

    Assert.Contains(pokemon.Changes, change => change is PokemonMoveLearned learned && learned.ActorId == actorId
      && learned.PowerPoints == powerPoints && learned.Position == position && !learned.TechnicalMachine);

    PokemonMove move = new(peck, powerPoints.Value, powerPoints.Value, powerPoints, IsMastered: false, pokemon.Level, TechnicalMachine: false);
    Assert.Equal(move, pokemon.AllMoves[move.MoveId]);
    Assert.Equal(Pokemon.MoveLimit, pokemon.Moves.Count);
    Assert.Equal(move, pokemon.Moves.ElementAt(position));

    Assert.Equal(5, pokemon.AllMoves.Count);
    Assert.True(pokemon.AllMoves.ContainsKey(tackle));
    Assert.True(pokemon.AllMoves.ContainsKey(growl));
    Assert.True(pokemon.AllMoves.ContainsKey(leafage));
    Assert.True(pokemon.AllMoves.ContainsKey(astonish));
    Assert.True(pokemon.AllMoves.ContainsKey(peck));
  }

  [Fact(DisplayName = "LearnMove: it should return false when the Pokémon has already learned the move.")]
  public void Given_MoveAlreadyLearned_When_LearnMove_Then_FalseReturned()
  {
    Pokemon pokemon = new(
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
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    pokemon.LearnMove(moveId, powerPoints);
    Assert.False(pokemon.LearnMove(moveId, powerPoints));
  }

  [Fact(DisplayName = "LearnMove: it should throw ArgumentNullException when the position is null and the Pokémon has reached the move limit.")]
  public void Given_MoveLimitReachedPositionNull_When_LearnMove_Then_ArgumentNullException()
  {
    Pokemon pokemon = new(
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
      experience: 419,
      vitality: int.MaxValue,
      stamina: int.MaxValue,
      friendship: 70);
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(35)); // NOTE(fpion): Tackle
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Growl
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(40)); // NOTE(fpion): Leafage
    pokemon.LearnMove(MoveId.NewId(), new PowerPoints(15)); // NOTE(fpion): Astonish
    var exception = Assert.Throws<ArgumentNullException>(() => pokemon.LearnMove(MoveId.NewId(), new PowerPoints(35))); // NOTE(fpion): Peck
    Assert.Equal("position", exception.ParamName);
  }

  [Theory(DisplayName = "LearnMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_LearnMove_Then_ArgumentOutOfRangeException(int position)
  {
    Pokemon pokemon = new(
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
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => pokemon.LearnMove(moveId, powerPoints, position));
    Assert.Equal("position", exception.ParamName);
  }

  [Fact(DisplayName = "MasterMove: it should master a move a Pokémon has learned.")]
  public void Given_MoveLearned_When_MasterMove_Then_MoveMastered()
  {
    Pokemon pokemon = new(
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

    MoveId leafage = MoveId.NewId();
    pokemon.LearnMove(leafage, new PowerPoints(40));

    ActorId actorId = ActorId.NewId();
    Assert.True(pokemon.MasterMove(leafage, actorId));
    Assert.True(pokemon.AllMoves[leafage].IsMastered);
    Assert.Contains(pokemon.Changes, change => change is PokemonMoveMastered mastered && mastered.ActorId == actorId && mastered.MoveId == leafage);

    pokemon.ClearChanges();
    Assert.True(pokemon.MasterMove(leafage));
    Assert.False(pokemon.HasChanges);
  }

  [Fact(DisplayName = "MasterMove: it should return false when the Pokémon has not learned the move yet.")]
  public void Given_NotLearned_When_MasterMove_Then_FalseReturned()
  {
    Pokemon pokemon = new(
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
    Assert.False(pokemon.MasterMove(MoveId.NewId()));
  }

  [Fact(DisplayName = "Receive: a trainer should be able to receive a Pokémon.")]
  public void Given_Trainer_When_Received_Then_Received()
  {
    ActorId actorId = ActorId.NewId();

    Pokemon pokemon = new(
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

    Assert.Null(pokemon.OriginalTrainerId);
    Assert.Null(pokemon.Ownership);

    TrainerId originalTrainerId = TrainerId.NewId();
    TrainerId currentTrainerId = TrainerId.NewId();
    ItemId pokeBallId = ItemId.NewId();
    ItemId greatBallId = ItemId.NewId();
    GameLocation originalLocation = new("Collège de l’Épervier");
    GameLocation currentLocation = new("Promenade Rivia");
    DateTime receivedOn = new(2000, 1, 1);
    pokemon.Receive(originalTrainerId, pokeBallId, originalLocation, receivedOn, description: null, actorId);

    PokemonOwnership ownership = new(originalTrainerId, pokeBallId, pokemon.Level, originalLocation, receivedOn, Description: null);
    Assert.Equal(ownership, pokemon.Ownership);
    Assert.Equal(originalTrainerId, pokemon.OriginalTrainerId);
    Assert.False(pokemon.IsTraded);
    Assert.Contains(pokemon.Changes, change => change is PokemonReceived received && received.ActorId == actorId && received.TrainerId == originalTrainerId
      && received.PokeBallId == pokeBallId && received.Level == pokemon.Level && received.Location == originalLocation && received.OccurredOn == receivedOn
      && received.Description is null);

    pokemon.ClearChanges();
    Description? description = new("Gifted by Jean-Guy Bowlpacker at Lv.9, at Promenade Rivia, on 2025 July, 8th.");
    pokemon.Receive(currentTrainerId, greatBallId, currentLocation, receivedOn: null, description, actorId);

    receivedOn = ((DomainEvent)pokemon.Changes.Single()).OccurredOn;
    ownership = new(currentTrainerId, pokeBallId, pokemon.Level, currentLocation, receivedOn, description);
    Assert.Equal(ownership, pokemon.Ownership);
    Assert.Equal(originalTrainerId, pokemon.OriginalTrainerId);
    Assert.True(pokemon.IsTraded);
    Assert.Contains(pokemon.Changes, change => change is PokemonReceived received && received.ActorId == actorId && received.TrainerId == currentTrainerId
      && received.PokeBallId == pokeBallId && received.Level == pokemon.Level && received.Location == currentLocation && received.Description == description);

    pokemon.ClearChanges();
    pokemon.Receive(currentTrainerId, greatBallId, currentLocation, receivedOn: null, description, actorId);
    Assert.Equal(ownership, pokemon.Ownership);
    Assert.Equal(originalTrainerId, pokemon.OriginalTrainerId);
    Assert.True(pokemon.IsTraded);
    Assert.False(pokemon.HasChanges);
  }

  [Fact(DisplayName = "RelearnMove: it should return false when the Pokémon has never learned the move.")]
  public void Given_NeverLearned_When_RelearnMove_Then_FalseReturned()
  {
    MoveId moveId = MoveId.NewId();
    Assert.False(_hedwidge.RelearnMove(moveId, position: 0));
  }

  [Fact(DisplayName = "RelearnMove: it should return false when the Pokémon has not forgot the move.")]
  public void Given_CurrentlyLearned_When_RelearnMove_Then_FalseReturned()
  {
    MoveId moveId = MoveId.NewId();
    _hedwidge.LearnMove(moveId, new PowerPoints(40));
    Assert.False(_hedwidge.RelearnMove(moveId, position: 0));
  }

  [Fact(DisplayName = "RelearnMove: it should relearn the specified move and return true.")]
  public void Given_ForgottenMove_When_RelearnMove_Then_MoveRelearned()
  {
    ActorId actorId = ActorId.NewId();

    MoveId tackle = MoveId.NewId();
    MoveId growl = MoveId.NewId();
    MoveId leafage = MoveId.NewId();
    MoveId astonish = MoveId.NewId();
    MoveId peck = MoveId.NewId();

    _hedwidge.LearnMove(tackle, new PowerPoints(35), actorId);
    _hedwidge.LearnMove(growl, new PowerPoints(40), actorId);
    _hedwidge.LearnMove(leafage, new PowerPoints(40), actorId);
    _hedwidge.LearnMove(astonish, new PowerPoints(15), actorId);
    _hedwidge.LearnMove(peck, new PowerPoints(35), position: 3, actorId);

    Assert.True(_hedwidge.RelearnMove(astonish, position: 1, actorId));
    Assert.Contains(_hedwidge.Changes, change => change is PokemonMoveRelearned relearned && relearned.ActorId == actorId
      && relearned.MoveId == astonish && relearned.Position == 1);

    Assert.Equal(tackle, _hedwidge.Moves.ElementAt(0).MoveId);
    Assert.Equal(astonish, _hedwidge.Moves.ElementAt(1).MoveId);
    Assert.Equal(leafage, _hedwidge.Moves.ElementAt(2).MoveId);
    Assert.Equal(peck, _hedwidge.Moves.ElementAt(3).MoveId);
  }

  [Theory(DisplayName = "RelearnMove: it should throw ArgumentOutOfRangeException when the position is out of bounds.")]
  [InlineData(-1)]
  [InlineData(4)]
  public void Given_PositionOutOfBounds_When_RelearnMove_Then_ArgumentOutOfRangeException(int position)
  {
    MoveId moveId = MoveId.NewId();
    PowerPoints powerPoints = new(35);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _hedwidge.RelearnMove(moveId, position));
    Assert.Equal("position", exception.ParamName);
  }

  [Fact(DisplayName = "Release: a trainer should be able to release its Pokémon.")]
  public void Given_Owner_When_Release_Then_Released()
  {
    Assert.Null(_hedwidge.OriginalTrainerId);
    Assert.Null(_hedwidge.Ownership);

    TrainerId trainerId = TrainerId.NewId();
    ItemId pokeBallId = ItemId.NewId();
    GameLocation location = new("Collège de l’Épervier");
    _hedwidge.Receive(trainerId, pokeBallId, location);
    _hedwidge.ClearChanges();

    Assert.NotNull(_hedwidge.OriginalTrainerId);
    Assert.NotNull(_hedwidge.Ownership);
    Assert.False(_hedwidge.IsTraded);

    ActorId actorId = ActorId.NewId();
    _hedwidge.Release(actorId);

    Assert.Null(_hedwidge.OriginalTrainerId);
    Assert.Null(_hedwidge.Ownership);
    Assert.False(_hedwidge.IsTraded);

    Assert.Contains(_hedwidge.Changes, change => change is PokemonReleased released && released.ActorId == actorId);
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

  // TODO(fpion): Gender
  // TODO(fpion): Sprite
  // TODO(fpion): Url
  // TODO(fpion): Notes
}
