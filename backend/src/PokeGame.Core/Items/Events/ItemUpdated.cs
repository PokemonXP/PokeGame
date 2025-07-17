using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Items.Events;

public record ItemUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<Price>? Price { get; set; }

  public Change<Url>? Sprite { get; set; }
  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
