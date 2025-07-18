using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Forms.Events;

public record FormCreated(
  VarietyId VarietyId,
  bool IsDefault,
  UniqueName UniqueName,
  bool IsBattleOnly,
  bool IsMega,
  Height Height,
  Weight Weight,
  FormTypes Types,
  FormAbilities Abilities,
  BaseStatistics BaseStatistics,
  Yield Yield,
  Sprites Sprites) : DomainEvent;
