using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Moves;

public class MoveNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified Pokémon move was not found.";

  public string Move
  {
    get => (string)Data[nameof(Move)]!;
    private set => Data[nameof(Move)] = value;
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
      error.Data[nameof(Move)] = Move;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public MoveNotFoundException(string move, string propertyName) : base(BuildMessage(move, propertyName))
  {
    Move = move;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string move, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Move), move)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
