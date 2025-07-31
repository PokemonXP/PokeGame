using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Storage.Events;

public record StoredPokemonSwapped(PokemonId SourceId, PokemonId DestinationId) : DomainEvent;
