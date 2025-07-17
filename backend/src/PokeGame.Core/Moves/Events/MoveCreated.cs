using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Moves.Events;

public record MoveCreated(PokemonType Type, MoveCategory Category, UniqueName UniqueName, Accuracy? Accuracy, Power? Power, PowerPoints PowerPoints) : DomainEvent;
