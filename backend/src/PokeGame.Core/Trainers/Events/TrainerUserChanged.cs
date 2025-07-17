using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace PokeGame.Core.Trainers.Events;

public record TrainerUserChanged(UserId? UserId) : DomainEvent;
