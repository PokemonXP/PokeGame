using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Trainers.Events;

public record TrainerCreated(License License, UniqueName UniqueName, TrainerGender Gender, Money Money) : DomainEvent;
