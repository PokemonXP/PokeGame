using Logitar.EventSourcing;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons;

namespace PokeGame.Core.Varieties.Events;

public record VarietyLevelMoveChanged(MoveId MoveId, Level Level) : DomainEvent;
