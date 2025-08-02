using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Battles.Models;

public record BattlerModel
{
  public PokemonModel Pokemon { get; set; } = new();
  public bool IsActive { get; set; }

  public BattleStatisticModel Attack { get; set; } = new();
  public BattleStatisticModel Defense { get; set; } = new();
  public BattleStatisticModel SpecialAttack { get; set; } = new();
  public BattleStatisticModel SpecialDefense { get; set; } = new();
  public BattleStatisticModel Speed { get; set; } = new();
  public int AccuracyStages { get; set; }
  public int EvasionStages { get; set; }
  public CriticalModel Critical { get; set; } = new();
}
