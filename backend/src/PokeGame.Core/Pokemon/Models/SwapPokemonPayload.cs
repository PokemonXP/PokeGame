namespace PokeGame.Core.Pokemon.Models;

public record SwapPokemonPayload
{
  public string Source { get; set; }
  public string Destination { get; set; }

  public SwapPokemonPayload() : this(string.Empty, string.Empty)
  {
  }

  public SwapPokemonPayload(string source, string destination)
  {
    Source = source;
    Destination = destination;
  }
}
