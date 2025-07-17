using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Trainers;

public class LicenseAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified trainer license is already used.";

  public Guid TrainerId
  {
    get => (Guid)Data[nameof(TrainerId)]!;
    private set => Data[nameof(TrainerId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string License
  {
    get => (string)Data[nameof(License)]!;
    private set => Data[nameof(License)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(TrainerId)] = TrainerId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(License)] = License;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public LicenseAlreadyUsedException(Trainer trainer, TrainerId conflictId)
    : base(BuildMessage(trainer, conflictId))
  {
    TrainerId = trainer.Id.ToGuid();
    ConflictId = conflictId.ToGuid();
    License = trainer.License.Value;
    PropertyName = nameof(trainer.License);
  }

  private static string BuildMessage(Trainer trainer, TrainerId conflictId) => new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(TrainerId), trainer.Id.ToGuid())
      .AddData(nameof(ConflictId), conflictId.ToGuid())
      .AddData(nameof(License), trainer.License)
      .AddData(nameof(PropertyName), nameof(trainer.License))
      .Build();
}
