using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Api.Models.Game;

public record StatisticsSummary
{
  public int HP { get; set; }
  public int Attack { get; set; }
  public int Defense { get; set; }
  public int SpecialAttack { get; set; }
  public int SpecialDefense { get; set; }
  public int Speed { get; set; }

  public StatisticsSummary()
  {
  }

  public StatisticsSummary(PokemonStatisticsModel statistics)
  {
    HP = statistics.HP.Value;
    Attack = statistics.Attack.Value;
    Defense = statistics.Defense.Value;
    SpecialAttack = statistics.SpecialAttack.Value;
    SpecialDefense = statistics.SpecialDefense.Value;
    Speed = statistics.Speed.Value;
  }
}
