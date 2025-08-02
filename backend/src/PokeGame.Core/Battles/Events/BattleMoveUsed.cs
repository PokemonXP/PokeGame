using Logitar.EventSourcing;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Events;

public record BattleMoveUsed(PokemonId AttackerId, MoveId MoveId, PokemonId TargetId, StatisticChanges StatisticChanges) : DomainEvent;
