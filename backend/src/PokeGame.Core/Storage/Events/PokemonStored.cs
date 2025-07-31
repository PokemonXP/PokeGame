using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Storage.Events;

public record PokemonStored(PokemonId PokemonId, PokemonSlot Slot) : DomainEvent;
