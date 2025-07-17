using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Species;

public class SpeciesNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified species was not found.";

  public string Species
  {
    get => (string)Data[nameof(Species)]!;
    private set => Data[nameof(Species)] = value;
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
      error.Data[nameof(Species)] = Species;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public SpeciesNotFoundException(string species, string propertyName) : base(BuildMessage(species, propertyName))
  {
    Species = species;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string species, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Species), species)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
