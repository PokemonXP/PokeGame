using Krakenar.Core;
using PokeGame.Core.Items;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemons;

public record PokemonOwnership(TrainerId TrainerId, ItemId PokeBallId, int Level, GameLocation Location, DateTime StartedOn, Description? Description);
