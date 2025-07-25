using PokeGame.Core;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Species.Models;

namespace PokeGame.Api.Models.Game;

public class PokemonSummary
{
  public Guid Id { get; set; }

  public int Number { get; set; }
  public string Name { get; set; }
  public string? Nickname { get; set; }
  public PokemonGender? Gender { get; set; }
  public string Sprite { get; set; }

  public double Height { get; set; }
  public double Weight { get; set; }
  public PokemonSizeCategory Size { get; set; }

  public PokemonType PrimaryType { get; set; }
  public PokemonType? SecondaryType { get; set; }
  public PokemonType TeraType { get; set; }

  public int Level { get; set; }
  public ExperienceSummary? Experience { get; set; }

  public ItemSummary? HeldItem { get; set; }

  public AbilitySummary? Ability { get; set; }
  public StatisticsSummary? Statistics { get; set; }
  public int Vitality { get; set; }
  public int Stamina { get; set; }
  public StatusCondition? StatusCondition { get; set; }

  public List<MoveSummary> Moves { get; set; } = [];

  public NatureSummary? Nature { get; set; }
  public TrainerSummary? OriginalTrainer { get; set; }
  public string? CaughtBallSprite { get; set; }
  public OwnershipSummary Ownership { get; set; }
  public string? Characteristic { get; set; }

  public PokemonSummary() : this(string.Empty, string.Empty)
  {
  }

  public PokemonSummary(string name, string sprite, OwnershipSummary? ownership = null)
  {
    Name = name;
    Sprite = sprite;
    Ownership = ownership ?? new();
  }

  public PokemonSummary(PokemonModel pokemon)
  {
    Id = pokemon.Id;

    Sprite = pokemon.GetSprite();

    OwnershipModel ownership = pokemon.Ownership ?? throw new ArgumentException($"The {nameof(pokemon.Ownership)} is required.", nameof(pokemon));
    Ownership = new OwnershipSummary(ownership);

    if (pokemon.IsEgg())
    {
      Name = "Egg";

      Ownership.Level = 0;
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

      double heightMultiplier = pokemon.Size.Height / 255.0 * 0.4 + 0.8;
      Height = Math.Floor(heightMultiplier * form.Height) / 10.0;
      Weight = Math.Floor((pokemon.Size.Weight / 255.0 * 0.4 + 0.8) * heightMultiplier * form.Weight) / 10.0;
      Size = pokemon.Size.Category;

      Level = pokemon.Level;
      Experience = new ExperienceSummary(pokemon);

      if (pokemon.HeldItem is not null)
      {
        HeldItem = new ItemSummary(pokemon.HeldItem);
      }

      switch (pokemon.AbilitySlot)
      {
        case AbilitySlot.Primary:
          Ability = new AbilitySummary(form.Abilities.Primary);
          break;
        case AbilitySlot.Secondary:
          if (form.Abilities.Secondary is not null)
          {
            Ability = new AbilitySummary(form.Abilities.Secondary);
          }
          break;
        case AbilitySlot.Hidden:
          if (form.Abilities.Hidden is not null)
          {
            Ability = new AbilitySummary(form.Abilities.Hidden);
          }
          break;
      }
      Statistics = new StatisticsSummary(pokemon.Statistics);
      Vitality = pokemon.Vitality;
      Stamina = pokemon.Stamina;
      StatusCondition = pokemon.StatusCondition;

      Moves.AddRange(pokemon.Moves.Where(m => m.Position.HasValue).OrderBy(m => m.Position!.Value).Select(m => new MoveSummary(m)));

      Nature = new NatureSummary(pokemon.Nature);
      OriginalTrainer = new TrainerSummary(ownership.OriginalTrainer);
      Characteristic = pokemon.Characteristic;
    }

    CaughtBallSprite = ownership.PokeBall.Sprite;
  }

  public override bool Equals(object? obj) => obj is PokemonSummary pokemon && pokemon.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Name} (Id={Id})";
}
