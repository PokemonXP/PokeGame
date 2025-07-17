using Logitar.EventSourcing;

namespace PokeGame.Core.Items.Events;

public record ItemDeleted : DomainEvent, IDeleteEvent;
