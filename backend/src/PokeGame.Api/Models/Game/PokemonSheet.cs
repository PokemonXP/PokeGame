using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Api.Models.Game;

public class PokemonSheet // TODO(fpion): should be named PokemonSummary...
{
  public Guid Id { get; set; }

  public string Name { get; set; }
  public PokemonGender? Gender { get; set; }
  public string Sprite { get; set; }

  public int Level { get; set; }
  public int Constitution { get; set; }
  public int Vitality { get; set; }
  public int Stamina { get; set; }

  public ItemSummary? HeldItem { get; set; }

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

    Name = pokemon.GetName();
    Sprite = pokemon.GetSprite();

    if (!pokemon.IsEgg())
    {
      Gender = pokemon.Gender;

      Level = pokemon.Level;
      Constitution = pokemon.Statistics.HP.Value;
      Vitality = pokemon.Vitality;
      Stamina = pokemon.Stamina;

      if (pokemon.HeldItem is not null)
      {
        HeldItem = new ItemSummary(pokemon.HeldItem);
      }
    }
  }

  public override bool Equals(object? obj) => obj is PokemonSheet pokemon && pokemon.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Name} (Id={Id})";
}
