using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonMoveSwitched(int Source, int Destination) : DomainEvent;
