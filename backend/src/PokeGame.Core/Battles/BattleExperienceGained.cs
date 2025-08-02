using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles;

public record BattleExperienceGained(PokemonId DefeatedId, PokemonId VictoriousId, int Experience) : DomainEvent;
