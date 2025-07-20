using Logitar.EventSourcing;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonMoveRemembered(MoveId MoveId, int Position) : DomainEvent;
