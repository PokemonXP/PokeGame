using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Trainers;

public class CannotEmptyTrainerPartyException : BadRequestException
{
  private const string ErrorMessage = "The trainer Pokémon party cannot be emptied.";

  public Guid TrainerId
  {
    get => (Guid)Data[nameof(TrainerId)]!;
    private set => Data[nameof(TrainerId)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(TrainerId)] = TrainerId;
      return error;
    }
  }

  public CannotEmptyTrainerPartyException(Trainer trainer) : this(trainer.Id)
  {
  }
  public CannotEmptyTrainerPartyException(TrainerId trainerId) : base(BuildMessage(trainerId))
  {
    TrainerId = trainerId.ToGuid();
  }

  private static string BuildMessage(TrainerId trainerId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TrainerId), trainerId.ToGuid())
    .Build();
}
