using Logitar.EventSourcing;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonMoveMastered(MoveId MoveId) : DomainEvent;
