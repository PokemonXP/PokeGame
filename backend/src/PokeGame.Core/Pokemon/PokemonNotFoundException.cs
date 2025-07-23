using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Pokemon;

public class PokemonNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified Pokémon were not found.";

  public IReadOnlyCollection<string> Pokemon
  {
    get => (IReadOnlyCollection<string>)Data[nameof(Pokemon)]!;
    private set => Data[nameof(Pokemon)] = value;
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
      error.Data[nameof(Pokemon)] = Pokemon;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public PokemonNotFoundException(IEnumerable<Guid> ids, string propertyName)
    : this(ids.Select(id => id.ToString()), propertyName)
  {
  }
  public PokemonNotFoundException(IEnumerable<string> pokemon, string propertyName)
    : base(BuildMessage(pokemon, propertyName))
  {
    Pokemon = pokemon.Distinct().ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<string> moves, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(Pokemon)).Append(':').AppendLine();
    foreach (string move in moves)
    {
      message.Append(" - ").AppendLine(move);
    }
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);

    return message.ToString();
  }
}
