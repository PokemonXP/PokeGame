using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Evolutions;

public class EvolutionNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified evolution was not found.";

  public string Evolution
  {
    get => (string)Data[nameof(Evolution)]!;
    private set => Data[nameof(Evolution)] = value;
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
      error.Data[nameof(Evolution)] = Evolution;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public EvolutionNotFoundException(string evolution, string propertyName) : base(BuildMessage(evolution, propertyName))
  {
    Evolution = evolution;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string evolution, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Evolution), evolution)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
