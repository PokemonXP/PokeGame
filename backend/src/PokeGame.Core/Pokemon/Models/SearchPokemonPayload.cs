using Krakenar.Contracts.Search;

namespace PokeGame.Core.Pokemon.Models;

public record SearchPokemonPayload : SearchPayload
{
  public Guid? SpeciesId { get; set; }
  public Guid? HeldItemId { get; set; }
  public Guid? TrainerId { get; set; }

  public bool? InParty { get; set; }
  public int? Box { get; set; }

  public new List<PokemonSortOption> Sort { get; set; } = [];
}
