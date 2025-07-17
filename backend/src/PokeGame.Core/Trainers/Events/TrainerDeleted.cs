using Logitar.EventSourcing;

namespace PokeGame.Core.Trainers.Events;

public record TrainerDeleted : DomainEvent, IDeleteEvent;
