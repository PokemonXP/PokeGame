using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Varieties.Events;

public record VarietyUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
