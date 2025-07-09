using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Items;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonReceived(TrainerId TrainerId, ItemId PokeBallId, int Level, GameLocation Location, Description? Description) : DomainEvent;
