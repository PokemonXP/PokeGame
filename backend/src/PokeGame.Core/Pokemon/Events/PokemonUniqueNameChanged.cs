using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
