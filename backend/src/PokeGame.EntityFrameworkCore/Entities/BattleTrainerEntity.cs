namespace PokeGame.EntityFrameworkCore.Entities;

internal class BattleTrainerEntity
{
  public BattleEntity? Battle { get; private set; }
  public int BattleId { get; private set; }
  public Guid BattleUid { get; private set; }

  public TrainerEntity? Trainer { get; private set; }
  public int TrainerId { get; private set; }
  public Guid TrainerUid { get; private set; }

  public bool IsOpponent { get; private set; }

  public BattleTrainerEntity(BattleEntity battle, TrainerEntity trainer, bool isOpponent)
  {
    Battle = battle;
    BattleId = battle.BattleId;
    BattleUid = battle.Id;

    Trainer = trainer;
    TrainerId = trainer.TrainerId;
    TrainerUid = trainer.Id;

    IsOpponent = isOpponent;
  }

  private BattleTrainerEntity()
  {
  }

  public override bool Equals(object? obj) => obj is BattleTrainerEntity entity && entity.BattleId == BattleId && entity.TrainerId == TrainerId;
  public override int GetHashCode() => HashCode.Combine(BattleId, TrainerId);
  public override string ToString() => $"{GetType()} (BattleId={BattleId}, TrainerId={TrainerId})";
}
