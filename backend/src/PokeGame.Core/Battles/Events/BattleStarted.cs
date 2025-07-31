using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Events;

public record BattleStarted(IReadOnlyCollection<PokemonId> PokemonIds) : DomainEvent;
