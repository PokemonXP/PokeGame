using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonSwapped(PokemonSlot Slot) : DomainEvent;
