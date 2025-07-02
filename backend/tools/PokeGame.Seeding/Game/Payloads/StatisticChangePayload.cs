using PokeGame.Core;

namespace PokeGame.Seeding.Game.Payloads;

public record StatisticChangePayload
{
  public PokemonStatistic Statistic { get; set; }
  public int Stages { get; set; }
}
