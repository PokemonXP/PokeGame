using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonHealed(int Healing, bool StatusCondition) : DomainEvent;
