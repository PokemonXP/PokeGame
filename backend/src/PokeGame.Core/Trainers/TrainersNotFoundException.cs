using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Trainers;

public class TrainersNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified trainers were not found.";

  public IReadOnlyCollection<string> Trainers
  {
    get => (IReadOnlyCollection<string>)Data[nameof(Trainers)]!;
    private set => Data[nameof(Trainers)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(Trainers)] = Trainers;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public TrainersNotFoundException(IEnumerable<string> trainers, string? propertyName = null)
    : base(BuildMessage(trainers, propertyName))
  {
    Trainers = trainers.Distinct().ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<string> trainers, string? propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(Trainers)).Append(':').AppendLine();
    foreach (string trainer in trainers)
    {
      message.Append(" - ").AppendLine(trainer);
    }
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");

    return message.ToString();
  }
}
