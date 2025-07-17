using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Regions;

public class RegionsNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified regions were not found.";

  public IReadOnlyCollection<string> Regions
  {
    get => (IReadOnlyCollection<string>)Data[nameof(Regions)]!;
    private set => Data[nameof(Regions)] = value;
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
      error.Data[nameof(Regions)] = Regions;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public RegionsNotFoundException(IEnumerable<string> regions, string propertyName)
    : base(BuildMessage(regions, propertyName))
  {
    Regions = regions.Distinct().ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<string> regions, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(Regions)).Append(':').AppendLine();
    foreach (string region in regions)
    {
      message.Append(" - ").AppendLine(region);
    }
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);

    return message.ToString();
  }
}
