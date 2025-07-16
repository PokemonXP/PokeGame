using Logitar.EventSourcing;

namespace PokeGame.Core.Regions.Events;

public record RegionDeleted : DomainEvent, IDeleteEvent;
