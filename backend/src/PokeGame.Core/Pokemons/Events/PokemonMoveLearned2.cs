using Logitar.EventSourcing;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonMoveLearned2(MoveId MoveId, PokemonMove2 Move, int? Position) : DomainEvent;
