using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Moves.Events;

public record MoveUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
