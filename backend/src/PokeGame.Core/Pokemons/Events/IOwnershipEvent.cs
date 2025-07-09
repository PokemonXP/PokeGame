using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemons.Events;

public interface IOwnershipEvent : ITemporalEvent
{
  OwnershipKind Kind { get; }
  int Level { get; }
  GameLocation Location { get; }
  Description? Description { get; }
}
