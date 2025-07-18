using PokeGame.Core.Items;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemons;

public record PokemonMove2(
  byte CurrentPowerPoints,
  byte MaximumPowerPoints,
  PowerPoints ReferencePowerPoints,
  bool IsMastered,
  Level Level,
  MoveLearningMethod Method,
  ItemId? ItemId,
  Notes? Notes);
