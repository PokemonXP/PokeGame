using Krakenar.Core;
using PokeGame.Core.Items;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Events;

public interface IPokemonOwnershipEvent
{
  TrainerId TrainerId { get; }
  ItemId PokeBallId { get; }
  Level Level { get; }
  Location Location { get; }
  DateTime? MetOn { get; }
  Description? Description { get; }
  PokemonSlot Slot { get; } // TASK: [POKEGAME-262](https://logitar.atlassian.net/browse/POKEGAME-262)
}
