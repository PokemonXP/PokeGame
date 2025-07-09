using Logitar.EventSourcing;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonMoveLearned(MoveId MoveId, PowerPoints PowerPoints, int? Position) : DomainEvent;
