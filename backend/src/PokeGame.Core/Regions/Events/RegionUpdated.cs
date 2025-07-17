using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Regions.Events;

public record RegionUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
