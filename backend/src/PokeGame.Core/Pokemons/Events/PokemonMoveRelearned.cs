using Logitar.EventSourcing;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonMoveRelearned(MoveId MoveId, int Position) : DomainEvent;
