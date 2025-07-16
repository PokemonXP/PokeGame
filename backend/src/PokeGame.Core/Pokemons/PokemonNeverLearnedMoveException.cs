using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;
using PokeGame.Core.Moves;

namespace PokeGame.Core.Pokemons;

public class PokemonNeverLearnedMoveException : BadRequestException
{
  private const string ErrorMessage = "The Pokémon cannot relearn the specified move because it has never been learned.";

  public Guid PokemonId
  {
    get => (Guid)Data[nameof(PokemonId)]!;
    private set => Data[nameof(PokemonId)] = value;
  }
  public Guid MoveId
  {
    get => (Guid)Data[nameof(MoveId)]!;
    private set => Data[nameof(MoveId)] = value;
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
      error.Data[nameof(MoveId)] = MoveId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public PokemonNeverLearnedMoveException(Pokemon pokemon, MoveId moveId, string propertyName)
    : base(BuildMessage(pokemon, moveId, propertyName))
  {
    PokemonId = pokemon.Id.ToGuid();
    MoveId = moveId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Pokemon pokemon, MoveId moveId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(PokemonId), pokemon.Id.ToGuid())
    .AddData(nameof(MoveId), moveId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
