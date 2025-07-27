using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonStored(PokemonId PokemonId, PokemonSlot Slot) : DomainEvent;
