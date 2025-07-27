using Krakenar.Core;
using PokeGame.Core.Items;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Events;

public interface IPokemonOwnershipEvent // TODO(fpion): remove Slot
{
  TrainerId TrainerId { get; }
  ItemId PokeBallId { get; }
  Level Level { get; }
  Location Location { get; }
  DateTime? MetOn { get; }
  Description? Description { get; }
  PokemonSlot Slot { get; }
}
