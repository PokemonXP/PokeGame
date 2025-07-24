using PokeGame.Core.Forms.Models;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Species.Models;

namespace PokeGame.Api.Models.Game;

public class PokemonSheet
{
  public Guid Id { get; set; }

  public string Name { get; set; }
  public PokemonGender? Gender { get; set; }
  public string Sprite { get; set; }

  public int Level { get; set; }
  public int Constitution { get; set; }
  public int Vitality { get; set; }
  public int Stamina { get; set; }

  public HeldItem? HeldItem { get; set; }

  public PokemonSheet() : this(string.Empty, string.Empty)
  {
  }

  public PokemonSheet(string name, string sprite)
  {
    Name = name;
    Sprite = sprite;
  }

  public PokemonSheet(PokemonModel pokemon)
  {
    Id = pokemon.Id;

    if (pokemon.EggCycles > 0)
    {
      Name = "Egg";
      Sprite = "/img/egg.png";
    }
    else
    {
      Name = GetName(pokemon);
      Gender = pokemon.Gender;
      Sprite = GetSprite(pokemon);

      Level = pokemon.Level;
      Constitution = pokemon.Statistics.HP.Value;
      Vitality = pokemon.Vitality;
      Stamina = pokemon.Stamina;

      if (pokemon.HeldItem is not null)
      {
        HeldItem = new HeldItem(pokemon.HeldItem);
      }
    }
  }

  private static string GetName(PokemonModel pokemon)
  {
    if (pokemon.Nickname is not null)
    {
      return pokemon.Nickname;
    }

    SpeciesModel species = pokemon.Form.Variety.Species;
    return species.DisplayName ?? species.UniqueName;
  }

  private static string GetSprite(PokemonModel pokemon)
  {
    if (pokemon.Sprite is not null)
    {
      return pokemon.Sprite;
    }

    FormModel form = pokemon.Form;
    if (pokemon.IsShiny)
    {
      return pokemon.Gender == PokemonGender.Female && form.Sprites.AlternativeShiny is not null ? form.Sprites.AlternativeShiny : form.Sprites.Shiny;
    }
    return pokemon.Gender == PokemonGender.Female && form.Sprites.Alternative is not null ? form.Sprites.Alternative : form.Sprites.Default;
  }

  public override bool Equals(object? obj) => obj is PokemonSheet pokemon && pokemon.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Name} (Id={Id})";
}
