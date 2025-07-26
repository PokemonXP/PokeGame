using Logitar.EventSourcing;
using PokeGame.Core.Forms;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonFormChanged(FormId FormId, BaseStatistics BaseStatistics) : DomainEvent;
