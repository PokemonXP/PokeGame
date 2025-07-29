using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Battles.Events;

public record TrainerBattleCreated(
  IReadOnlyCollection<TrainerId> ChampionIds,
  IReadOnlyCollection<TrainerId> OpponentIds,
  DisplayName Name,
  Location Location,
  Url? Url,
  Notes? Notes) : DomainEvent, IBattleCreated;
