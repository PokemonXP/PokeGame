using Krakenar.Core;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemons;

[Trait(Traits.Category, Categories.Unit)]
public class OwnershipTests
{
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly Pokemon _hedwidge;

  public OwnershipTests()
  {
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

  [Theory(DisplayName = "Catch: it should catch a wild Pokémon.")]
  [InlineData(false, null)]
  [InlineData(true, "Caught at Lv.5, at Parc Frandelys, on 2000-01-01.")]
  public void Given_Wild_When_Catch_Then_Caught(bool hasDate, string? descriptionText)
  {
    TrainerId trainerId = TrainerId.NewId();
    ItemId pokeBallId = ItemId.NewId();
    GameLocation location = new("Parc Frandelys");
    DateTime? caughtOn = hasDate ? new DateTime(2000, 1, 1) : null;
    Description? description = Description.TryCreate(descriptionText);
    ActorId actorId = ActorId.NewId();

    Assert.True(_hedwidge.Catch(trainerId, pokeBallId, location, caughtOn, description, actorId));

    caughtOn ??= _hedwidge.UpdatedOn;
    PokemonOwnership ownership = new(OwnershipKind.Caught, trainerId, pokeBallId, _hedwidge.Level, location, caughtOn.Value, description);
    Assert.Equal(ownership, _hedwidge.Ownership);
    Assert.Equal(trainerId, _hedwidge.OriginalTrainerId);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonCaught caught && caught.ActorId == actorId
      && caught.OccurredOn == caughtOn && caught.TrainerId == trainerId && caught.PokeBallId == pokeBallId
      && caught.Level == _hedwidge.Level && caught.Location == location && caught.Description == description);
  }

  [Fact(DisplayName = "Catch: it should return false when the Pokémon has an owner.")]
  public void Given_Owned_When_Catch_Then_FalseReturned()
  {
    TrainerId trainerId = TrainerId.NewId();
    ItemId pokeBallId = ItemId.NewId();
    GameLocation location = new("Parc Frandelys");
    _hedwidge.Receive(trainerId, pokeBallId, location);

    Assert.False(_hedwidge.Catch(trainerId, pokeBallId, location));
  }

  [Fact(DisplayName = "Receive: a trainer should be able to receive a Pokémon.")]
  public void Given_Trainer_When_Received_Then_Received()
  {
    ActorId actorId = ActorId.NewId();

    Assert.Null(_hedwidge.OriginalTrainerId);
    Assert.Null(_hedwidge.Ownership);

    TrainerId originalTrainerId = TrainerId.NewId();
    TrainerId currentTrainerId = TrainerId.NewId();
    ItemId pokeBallId = ItemId.NewId();
    ItemId greatBallId = ItemId.NewId();
    GameLocation originalLocation = new("Collège de l’Épervier");
    GameLocation currentLocation = new("Promenade Rivia");
    DateTime receivedOn = new(2000, 1, 1);
    _hedwidge.Receive(originalTrainerId, pokeBallId, originalLocation, receivedOn, description: null, actorId);

    PokemonOwnership ownership = new(OwnershipKind.Received, originalTrainerId, pokeBallId, _hedwidge.Level, originalLocation, receivedOn, Description: null);
    Assert.Equal(ownership, _hedwidge.Ownership);
    Assert.Equal(originalTrainerId, _hedwidge.OriginalTrainerId);
    Assert.False(_hedwidge.IsTraded);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonReceived received && received.ActorId == actorId && received.TrainerId == originalTrainerId
      && received.PokeBallId == pokeBallId && received.Level == _hedwidge.Level && received.Location == originalLocation && received.OccurredOn == receivedOn
      && received.Description is null);

    _hedwidge.ClearChanges();
    Description? description = new("Gifted by Jean-Guy Bowlpacker at Lv.9, at Promenade Rivia, on 2025-07-08.");
    _hedwidge.Receive(currentTrainerId, greatBallId, currentLocation, receivedOn: null, description, actorId);

    receivedOn = ((DomainEvent)_hedwidge.Changes.Single()).OccurredOn;
    ownership = new(OwnershipKind.Received, currentTrainerId, pokeBallId, _hedwidge.Level, currentLocation, receivedOn, description);
    Assert.Equal(ownership, _hedwidge.Ownership);
    Assert.Equal(originalTrainerId, _hedwidge.OriginalTrainerId);
    Assert.True(_hedwidge.IsTraded);
    Assert.Contains(_hedwidge.Changes, change => change is PokemonReceived received && received.ActorId == actorId && received.TrainerId == currentTrainerId
      && received.PokeBallId == pokeBallId && received.Level == _hedwidge.Level && received.Location == currentLocation && received.Description == description);

    _hedwidge.ClearChanges();
    _hedwidge.Receive(currentTrainerId, greatBallId, currentLocation, receivedOn: null, description, actorId);
    Assert.Equal(ownership, _hedwidge.Ownership);
    Assert.Equal(originalTrainerId, _hedwidge.OriginalTrainerId);
    Assert.True(_hedwidge.IsTraded);
    Assert.False(_hedwidge.HasChanges);
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
}
