using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonMoved(PokemonSlot Slot) : DomainEvent;
