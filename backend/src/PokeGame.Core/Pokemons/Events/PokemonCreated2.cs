using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonCreated2(
  SpeciesId SpeciesId,
  VarietyId VarietyId,
  FormId FormId,
  UniqueName UniqueName,
  PokemonGender? Gender,
  bool IsShiny,
  PokemonType TeraType,
  PokemonSize Size,
  AbilitySlot AbilitySlot,
  PokemonNature Nature,
  GrowthRate GrowthRate,
  int Experience,
  BaseStatistics BaseStatistics,
  IndividualValues IndividualValues,
  EffortValues EffortValues,
  int Vitality,
  int Stamina,
  Friendship Friendship) : DomainEvent;
