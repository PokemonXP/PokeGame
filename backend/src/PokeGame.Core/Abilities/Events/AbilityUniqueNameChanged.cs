using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Abilities.Events;

public record AbilityUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
