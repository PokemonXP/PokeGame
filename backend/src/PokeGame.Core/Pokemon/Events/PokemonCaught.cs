using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Items;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonCaught(TrainerId TrainerId, ItemId PokeBallId, Level Level, Location Location, DateTime? MetOn, Description? Description, PokemonSlot Slot)
  : DomainEvent, IPokemonOwnershipEvent; // TODO(fpion): remove Slot
