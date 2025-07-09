using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemons;

public record PokemonMove(
  MoveId MoveId,
  int CurrentPowerPoints,
  int MaximumPowerPoints,
  PowerPoints ReferencePowerPoints,
  bool IsMastered,
  int Level,
  bool TechnicalMachine);
