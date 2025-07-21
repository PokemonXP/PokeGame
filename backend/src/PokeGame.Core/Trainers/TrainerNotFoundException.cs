using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Trainers;

public class TrainerNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified trainer was not found.";

  public string Trainer
  {
    get => (string)Data[nameof(Trainer)]!;
    private set => Data[nameof(Trainer)] = value;
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
      error.Data[nameof(Trainer)] = Trainer;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public TrainerNotFoundException(string trainer, string propertyName) : base(BuildMessage(trainer, propertyName))
  {
    Trainer = trainer;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string trainer, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Trainer), trainer)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
