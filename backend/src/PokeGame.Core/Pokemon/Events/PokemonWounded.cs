using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonWounded(int Damage, StatusCondition? StatusCondition) : DomainEvent;
