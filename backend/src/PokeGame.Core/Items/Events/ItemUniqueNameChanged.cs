using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Items.Events;

public record ItemUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
