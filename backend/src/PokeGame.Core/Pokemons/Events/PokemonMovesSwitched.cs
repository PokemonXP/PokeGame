using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonMovesSwitched(int Source, int Destination) : DomainEvent;
