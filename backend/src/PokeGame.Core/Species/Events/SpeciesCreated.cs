using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Species.Events;

public record SpeciesCreated(Number Number, PokemonCategory Category, UniqueName UniqueName, Friendship BaseFriendship, CatchRate CatchRate, GrowthRate GrowthRate)
  : DomainEvent;
