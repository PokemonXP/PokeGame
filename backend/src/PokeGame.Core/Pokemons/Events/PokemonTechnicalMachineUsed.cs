using Logitar.EventSourcing;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonTechnicalMachineUsed(MoveId MoveId, PowerPoints PowerPoints, int? Position) : DomainEvent;
