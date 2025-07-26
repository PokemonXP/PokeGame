using Logitar.EventSourcing;

namespace PokeGame.Core.Evolutions.Events;

public record EvolutionDeleted : DomainEvent, IDeleteEvent;
