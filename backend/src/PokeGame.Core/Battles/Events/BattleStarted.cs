using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;

namespace PokeGame.Core.Battles.Events;

/// <summary>
/// The dictionary represents Pokémon status in battle. When value is true, the Pokémon is currently battling.
/// </summary>
public record BattleStarted(IReadOnlyDictionary<PokemonId, bool> PokemonIds) : DomainEvent;
