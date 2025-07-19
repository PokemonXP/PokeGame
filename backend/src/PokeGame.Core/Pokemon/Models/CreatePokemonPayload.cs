using PokeGame.Core.Abilities;

namespace PokeGame.Core.Pokemon.Models;

public record CreatePokemonPayload
{
  public Guid? Id { get; set; }

  public string Form { get; set; } = string.Empty;

  public string? UniqueName { get; set; }
  public string? Nickname { get; set; }
  public PokemonGender? Gender { get; set; }
  public bool IsShiny { get; set; }

  public PokemonType? TeraType { get; set; }
  public PokemonSizePayload? Size { get; set; }
  public AbilitySlot? AbilitySlot { get; set; }
  public string? Nature { get; set; }

  public byte EggCycles { get; set; }
  public int Experience { get; set; }

  public IndividualValuesModel? IndividualValues { get; set; }
  public EffortValuesModel? EffortValues { get; set; }
  public int? Vitality { get; set; }
  public int? Stamina { get; set; }
  public byte? Friendship { get; set; }

  public string? HeldItem { get; set; }

  public string? Sprite { get; set; }
  public string? Url { get; set; }
  public string? Notes { get; set; }
}
