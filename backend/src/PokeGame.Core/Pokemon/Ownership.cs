using Krakenar.Core;
using PokeGame.Core.Items;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon;

public record Ownership(OwnershipKind Kind, TrainerId TrainerId, ItemId PokeBallId, Level Level, Location Location, DateTime MetOn, Description? Description);
