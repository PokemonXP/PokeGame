using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Events;

public record BattleReset(IReadOnlyCollection<PokemonId> OpponentIds) : DomainEvent;
