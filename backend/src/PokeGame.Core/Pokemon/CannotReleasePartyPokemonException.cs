﻿using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;

namespace PokeGame.Core.Pokemon;

public class CannotReleasePartyPokemonException : BadRequestException
{
  private const string ErrorMessage = "A Pokémon in the party cannot be released.";

  public Guid PokemonId
  {
    get => (Guid)Data[nameof(PokemonId)]!;
    private set => Data[nameof(PokemonId)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(PokemonId)] = PokemonId;
      return error;
    }
  }

  public CannotReleasePartyPokemonException(Specimen pokemon) : base(BuildMessage(pokemon))
  {
    PokemonId = pokemon.Id.ToGuid();
  }

  private static string BuildMessage(Specimen pokemon) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(PokemonId), pokemon.Id.ToGuid())
    .Build();
}
