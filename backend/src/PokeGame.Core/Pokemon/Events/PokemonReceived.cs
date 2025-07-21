using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Items;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonReceived(TrainerId TrainerId, ItemId PokeBallId, Level Level, Location Location, Description? Description, Position Position, Box? Box)
  : DomainEvent;
