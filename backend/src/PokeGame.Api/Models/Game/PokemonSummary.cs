using PokeGame.Core;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Species.Models;

namespace PokeGame.Api.Models.Game;

public class PokemonSummary // TODO(fpion): should be named PokemonSheet...
{
  public Guid Id { get; set; }

  public int Number { get; set; }
  public string Name { get; set; }
  public string? Nickname { get; set; }
  public PokemonGender? Gender { get; set; }
  public string Sprite { get; set; }

  public PokemonType PrimaryType { get; set; }
  public PokemonType? SecondaryType { get; set; }
  public PokemonType TeraType { get; set; }

  public int Level { get; set; }
  public int Experience { get; set; }

  public ItemSummary? HeldItem { get; set; }

  public TrainerSummary? OriginalTrainer { get; set; }
  public ItemSummary PokeBall { get; set; }

  public PokemonSummary() : this(string.Empty, string.Empty, new ItemSummary())
  {
  }

  public PokemonSummary(string name, string sprite, ItemSummary pokeBall)
  {
    Name = name;
    Sprite = sprite;

    PokeBall = pokeBall;
  }

  public PokemonSummary(PokemonModel pokemon)
  {
    OwnershipModel ownership = pokemon.Ownership ?? throw new ArgumentException($"The {nameof(pokemon.Ownership)} is required.", nameof(pokemon));

    Id = pokemon.Id;

    Sprite = pokemon.GetSprite();

    if (pokemon.IsEgg())
    {
      Name = "Egg";
    }
    else
    {
      FormModel form = pokemon.Form;
      SpeciesModel species = form.Variety.Species;

      Number = pokemon.Form.Variety.Species.Number;
      Name = species.DisplayName ?? species.UniqueName;
      Nickname = pokemon.Nickname;
      Gender = pokemon.Gender;

      PrimaryType = form.Types.Primary;
      SecondaryType = form.Types.Secondary;
      TeraType = pokemon.TeraType;

      Level = pokemon.Level;
      Experience = pokemon.Experience;

      if (pokemon.HeldItem is not null)
      {
        HeldItem = new ItemSummary(pokemon.HeldItem);
      }

      OriginalTrainer = new TrainerSummary(ownership.OriginalTrainer);
    }

    PokeBall = new ItemSummary(ownership.PokeBall);
  }

  public override bool Equals(object? obj) => obj is PokemonSummary pokemon && pokemon.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Name} (Id={Id})";
}
