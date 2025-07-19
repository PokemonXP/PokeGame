using Logitar.EventSourcing;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonMoveLearned(MoveId MoveId, PokemonMove Move, int? Position) : DomainEvent;
