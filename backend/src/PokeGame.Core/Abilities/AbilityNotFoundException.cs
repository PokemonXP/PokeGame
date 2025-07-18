using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Abilities;

public class AbilityNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified ability was not found.";

  public string Ability
  {
    get => (string)Data[nameof(Ability)]!;
    private set => Data[nameof(Ability)] = value;
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
      error.Data[nameof(Ability)] = Ability;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public AbilityNotFoundException(string ability, string propertyName) : base(BuildMessage(ability, propertyName))
  {
    Ability = ability;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string ability, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Ability), ability)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
