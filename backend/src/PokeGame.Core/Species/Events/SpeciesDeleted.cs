using Logitar.EventSourcing;

namespace PokeGame.Core.Species.Events;

public record SpeciesDeleted : DomainEvent, IDeleteEvent;
