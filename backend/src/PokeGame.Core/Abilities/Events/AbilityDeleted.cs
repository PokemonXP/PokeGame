using Logitar.EventSourcing;

namespace PokeGame.Core.Abilities.Events;

public record AbilityDeleted : DomainEvent, IDeleteEvent;
