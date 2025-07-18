using Logitar.EventSourcing;

namespace PokeGame.Core.Forms.Events;

public record FormDeleted : DomainEvent, IDeleteEvent;
