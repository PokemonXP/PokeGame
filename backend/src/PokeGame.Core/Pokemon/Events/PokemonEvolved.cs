using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonEvolved(SpeciesId SpeciesId, VarietyId VarietyId, FormId FormId, BaseStatistics BaseStatistics) : DomainEvent;
