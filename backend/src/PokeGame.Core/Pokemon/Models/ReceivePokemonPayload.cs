namespace PokeGame.Core.Pokemon.Models;

public record ReceivePokemonPayload
{
  public string Trainer { get; set; } = string.Empty;
  public string PokeBall { get; set; } = string.Empty;

  public int Level { get; set; }
  public string Location { get; set; } = string.Empty;
  public DateTime? MetOn { get; set; }
  public string? Description { get; set; }
}
