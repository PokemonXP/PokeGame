using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonUpdated : DomainEvent
{
  public Change<Url>? Sprite { get; set; }
  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
