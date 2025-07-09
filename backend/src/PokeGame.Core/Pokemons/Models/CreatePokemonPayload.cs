namespace PokeGame.Core.Pokemons.Models;

public record CreatePokemonPayload
{
  public Guid? Id { get; set; }

  public string Form { get; set; } = string.Empty;

  public string UniqueName { get; set; } = string.Empty;
  public string? Nickname { get; set; }
  public PokemonGender? Gender { get; set; }

  public PokemonType? TeraType { get; set; }
  public PokemonSize? Size { get; set; }
  public AbilitySlot? AbilitySlot { get; set; }
  public string? Nature { get; set; }

  public int Experience { get; set; }

  public IndividualValuesModel? IndividualValues { get; set; }
  public EffortValuesModel? EffortValues { get; set; }
  public int Vitality { get; set; }
  public int Stamina { get; set; }
  // TODO(fpion): StatusCondition

  public byte? Friendship { get; set; }

  // TODO(fpion): HeldItemId

  // TODO(fpion): Moves

  // TODO(fpion): OriginalTrainer
  // TODO(fpion): Ownership

  public string? Sprite { get; set; }
  public string? Url { get; set; }
  public string? Notes { get; set; }
}
