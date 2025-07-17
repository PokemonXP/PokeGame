using Logitar.EventSourcing;

namespace PokeGame.Core.Varieties.Events;

public record VarietyDeleted : DomainEvent, IDeleteEvent;
