using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonMoveSwapped(int Source, int Destination) : DomainEvent;
