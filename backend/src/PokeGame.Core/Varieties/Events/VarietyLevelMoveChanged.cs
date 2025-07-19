using Logitar.EventSourcing;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Varieties.Events;

public record VarietyLevelMoveChanged(MoveId MoveId, Level Level) : DomainEvent;
