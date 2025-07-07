using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Pokemons;

public class PokemonUniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified Pokémon unique name is already used.";

  public Guid PokemonId
  {
    get => (Guid)Data[nameof(PokemonId)]!;
    private set => Data[nameof(PokemonId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
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
      error.Data[nameof(PokemonId)] = PokemonId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public PokemonUniqueNameAlreadyUsedException(Pokemon pokemon, PokemonId conflictId) : base(BuildMessage(pokemon, conflictId))
  {
    PokemonId = pokemon.Id.ToGuid();
    ConflictId = conflictId.ToGuid();
    UniqueName = pokemon.UniqueName.Value;
    PropertyName = nameof(pokemon.UniqueName);
  }

  private static string BuildMessage(Pokemon pokemon, PokemonId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(PokemonId), pokemon.Id.ToGuid())
    .AddData(nameof(ConflictId), conflictId.ToGuid())
    .AddData(nameof(UniqueName), pokemon.UniqueName)
    .AddData(nameof(PropertyName), nameof(pokemon.UniqueName))
    .Build();
}
