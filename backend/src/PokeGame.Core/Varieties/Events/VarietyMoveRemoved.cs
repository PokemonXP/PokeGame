using Logitar.EventSourcing;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Varieties.Events;

public record VarietyMoveRemoved(MoveId MoveId) : DomainEvent;
