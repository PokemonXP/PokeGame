namespace PokeGame.Core.Battles;

public record Battler
{
  public bool IsActive { get; }
  public StatisticChanges StatisticChanges { get; }

  public Battler(bool isActive = true, StatisticChanges? statisticChanges = null)
  {
    IsActive = isActive;
    StatisticChanges = statisticChanges ?? new();
  }

  public Battler Apply(StatisticChanges statisticChanges) => IsActive ? new(IsActive, StatisticChanges.Apply(statisticChanges)) : this;

  public Battler Switch() => new(!IsActive);
}
