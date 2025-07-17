using Logitar.EventSourcing;

namespace PokeGame.Core.Moves.Events;

public record MoveDeleted : DomainEvent, IDeleteEvent;
