using Logitar.EventSourcing;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonMoveUsed(MoveId MoveId, PowerPoints? PowerPointCost, int StaminaCost) : DomainEvent;
