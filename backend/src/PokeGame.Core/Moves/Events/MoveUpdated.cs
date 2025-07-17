using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Moves.Events;

public record MoveUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<Accuracy>? Accuracy { get; set; }
  public Change<Power>? Power { get; set; }
  public PowerPoints? PowerPoints { get; set; }

  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
