using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonDeleted : DomainEvent, IDeleteEvent;
