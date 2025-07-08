using Logitar.EventSourcing;
using PokeGame.Core.Items;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonItemHeld(ItemId ItemId) : DomainEvent;
