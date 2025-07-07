using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonCreated(
  FormId FormId,
  UniqueName UniqueName,
  PokemonGender? Gender,
  PokemonType TeraType,
  PokemonSize Size,
  AbilitySlot AbilitySlot,
  Nature Nature,
  IndividualValues IndividualValues,
  EffortValues EffortValues,
  int Experience,
  int Vitality,
  int Stamina,
  byte Friendship) : DomainEvent;
