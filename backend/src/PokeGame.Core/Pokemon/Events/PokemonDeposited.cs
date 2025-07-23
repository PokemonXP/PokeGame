using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonDeposited(PokemonSlot Slot) : DomainEvent;
