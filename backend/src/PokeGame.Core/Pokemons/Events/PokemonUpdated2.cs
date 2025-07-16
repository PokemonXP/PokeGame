using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonUpdated2 : DomainEvent
{
  public Change<Url>? Sprite { get; set; }
  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
