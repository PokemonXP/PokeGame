using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Regions.Events;

public record RegionUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
