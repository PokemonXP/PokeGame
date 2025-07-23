namespace PokeGame.Core.Pokemon.Models;

public record SwapPokemonPayload
{
  public List<Guid> Ids { get; set; } = [];
}
