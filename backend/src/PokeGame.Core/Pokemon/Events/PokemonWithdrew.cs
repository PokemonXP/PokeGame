using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonWithdrew(PokemonSlot Slot) : DomainEvent;
