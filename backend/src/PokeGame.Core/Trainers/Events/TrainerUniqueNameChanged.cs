using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Trainers.Events;

public record TrainerUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
