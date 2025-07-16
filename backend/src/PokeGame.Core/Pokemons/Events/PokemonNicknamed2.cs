using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonNicknamed2(Nickname? Nickname) : DomainEvent;
