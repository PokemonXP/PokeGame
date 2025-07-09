using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;
using System.Text;

namespace PokeGame.Core.Moves;

public class MovesNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified Pokémon moves were not found.";

  public IReadOnlyCollection<string> Moves
  {
    get => (IReadOnlyCollection<string>)Data[nameof(Moves)]!;
    private set => Data[nameof(Moves)] = value;
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
      error.Data[nameof(Moves)] = Moves;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public MovesNotFoundException(IEnumerable<string> moves, string propertyName) : base(BuildMessage(moves, propertyName))
  {
    Moves = moves.ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<string> moves, string propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(Moves)).Append(':').AppendLine();
    foreach (string move in moves)
    {
      message.Append(" - ").AppendLine(move);
    }
    return message.ToString();
  }
}
