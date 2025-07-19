using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonNicknamed(Nickname? Nickname) : DomainEvent;
