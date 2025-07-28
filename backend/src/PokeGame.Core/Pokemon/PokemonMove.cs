using PokeGame.Core.Items;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemon;

public record PokemonMove(
  byte CurrentPowerPoints,
  byte MaximumPowerPoints,
  PowerPoints ReferencePowerPoints,
  bool IsMastered,
  Level Level,
  MoveLearningMethod Method,
  ItemId? ItemId,
  Notes? Notes)
{
  public PokemonMove RestorePowerPoints()
  {
    return new PokemonMove(CurrentPowerPoints: MaximumPowerPoints, MaximumPowerPoints, ReferencePowerPoints, IsMastered, Level, Method, ItemId, Notes);
  }
}
