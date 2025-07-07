using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
