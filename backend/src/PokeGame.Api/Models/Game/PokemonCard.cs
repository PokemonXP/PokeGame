using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Api.Models.Game;

public class PokemonCard
{
  public Guid Id { get; set; }

  public string Name { get; set; }
  public PokemonGender? Gender { get; set; }
  public string Sprite { get; set; }

  public int Level { get; set; }
  public int Constitution { get; set; }
  public int Vitality { get; set; }
  public int Stamina { get; set; }

  public ItemCard? HeldItem { get; set; }

  public PokemonCard() : this(string.Empty, string.Empty)
  {
  }

  public PokemonCard(string name, string sprite)
  {
    Name = name;
    Sprite = sprite;
  }

  public PokemonCard(PokemonModel pokemon)
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
        HeldItem = new ItemCard(pokemon.HeldItem);
      }
    }
  }

  public override bool Equals(object? obj) => obj is PokemonCard pokemon && pokemon.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Name} (Id={Id})";
}
