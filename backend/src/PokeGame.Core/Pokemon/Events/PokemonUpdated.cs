using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemon.Events;

public record PokemonUpdated : DomainEvent
{
  public bool? IsShiny { get; set; }

  public int? Vitality { get; set; }
  public int? Stamina { get; set; }
  public Change<StatusCondition?>? StatusCondition { get; set; }
  public Friendship? Friendship { get; set; }

  public Change<Url>? Sprite { get; set; }
  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
