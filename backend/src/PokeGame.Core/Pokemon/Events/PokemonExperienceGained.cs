using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonExperienceGained(int Experience) : DomainEvent;
