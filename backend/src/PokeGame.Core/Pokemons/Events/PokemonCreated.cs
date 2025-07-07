using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Species;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonCreated(
  FormId FormId,
  UniqueName UniqueName,
  PokemonGender? Gender,
  PokemonType TeraType,
  PokemonSize Size,
  AbilitySlot AbilitySlot,
  PokemonNature Nature,
  IndividualValues IndividualValues,
  EffortValues EffortValues,
  GrowthRate GrowthRate,
  int Experience,
  int Vitality,
  int Stamina,
  byte Friendship) : DomainEvent;
