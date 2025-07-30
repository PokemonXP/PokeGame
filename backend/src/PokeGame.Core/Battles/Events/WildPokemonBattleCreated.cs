using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Battles.Events;

public record WildPokemonBattleCreated(
  IReadOnlyCollection<TrainerId> ChampionIds,
  IReadOnlyCollection<PokemonId> OpponentIds,
  DisplayName Name,
  Location Location,
  Url? Url,
  Notes? Notes) : DomainEvent, IBattleCreated;
