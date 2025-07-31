using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Storage.Events;

public record PokemonRemoved(PokemonId PokemonId) : DomainEvent;
