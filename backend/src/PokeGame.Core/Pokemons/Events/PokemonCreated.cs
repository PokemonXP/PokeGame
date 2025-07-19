using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemon;
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
  GrowthRate GrowthRate,
  int Experience,
  BaseStatistics BaseStatistics,
  IndividualValues IndividualValues,
  EffortValues EffortValues,
  int Vitality,
  int Stamina,
  PokemonCharacteristic Characteristic,
  byte Friendship) : DomainEvent;
