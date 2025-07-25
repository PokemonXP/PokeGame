using PokeGame.Core.Forms.Models;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Species.Models;

namespace PokeGame.Api.Models.Game;

internal static class SheetExtensions
{
  public static string GetName(this PokemonModel pokemon)
  {
    if (pokemon.IsEgg)
    {
      return "Egg";
    }
    else if (pokemon.Nickname is not null)
    {
      return pokemon.Nickname;
    }

    SpeciesModel species = pokemon.Form.Variety.Species;
    return species.DisplayName ?? species.UniqueName;
  }

  public static string GetSprite(this PokemonModel pokemon)
  {
    if (pokemon.IsEgg)
    {
      return "/img/egg.png";
    }
    else if (pokemon.Sprite is not null)
    {
      return pokemon.Sprite;
    }

    FormModel form = pokemon.Form;
    return pokemon.IsShiny
      ? (pokemon.Gender == PokemonGender.Female && form.Sprites.AlternativeShiny is not null ? form.Sprites.AlternativeShiny : form.Sprites.Shiny)
      : (pokemon.Gender == PokemonGender.Female && form.Sprites.Alternative is not null ? form.Sprites.Alternative : form.Sprites.Default);
  }
}
