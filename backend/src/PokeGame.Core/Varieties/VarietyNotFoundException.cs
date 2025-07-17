using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Varieties;

public class VarietyNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified variety was not found.";

  public string Variety
  {
    get => (string)Data[nameof(Variety)]!;
    private set => Data[nameof(Variety)] = value;
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
      error.Data[nameof(Variety)] = Variety;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public VarietyNotFoundException(string variety, string propertyName) : base(BuildMessage(variety, propertyName))
  {
    Variety = variety;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string variety, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Variety), variety)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
