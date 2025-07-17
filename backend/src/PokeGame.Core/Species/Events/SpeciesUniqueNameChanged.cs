using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Species.Events;

public record SpeciesUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
