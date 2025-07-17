using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Abilities.Events;

public record AbilityCreated(UniqueName UniqueName) : DomainEvent;
