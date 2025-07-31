using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Events;

public record BattlePokemonSwitched(PokemonId ActiveId, PokemonId InactiveId) : DomainEvent;
