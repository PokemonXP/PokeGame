using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemons;

public record PokemonMove(
  MoveId MoveId,
  int CurrentPowerPoints,
  int MaximumPowerPoints,
  PowerPoints ReferencePowerPoints,
  bool IsMastered,
  int Level,
  bool TechnicalMachine)
{
  public PokemonMove Master() => new(MoveId, CurrentPowerPoints, MaximumPowerPoints, ReferencePowerPoints, IsMastered: true, Level, TechnicalMachine);
};
