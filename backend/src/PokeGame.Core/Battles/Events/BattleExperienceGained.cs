using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Events;

public record BattleExperienceGained(PokemonId DefeatedId, PokemonId VictoriousId, int Experience) : DomainEvent;
