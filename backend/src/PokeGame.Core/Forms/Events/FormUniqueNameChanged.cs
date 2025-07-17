using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Forms.Events;

public record FormUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
