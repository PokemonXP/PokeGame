using Krakenar.Core;
using PokeGame.Core.Items;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemons;

public record PokemonOwnership(
  OwnershipKind Kind,
  TrainerId TrainerId,
  ItemId PokeBallId,
  int Level,
  GameLocation Location,
  DateTime MetOn,
  Description? Description);
