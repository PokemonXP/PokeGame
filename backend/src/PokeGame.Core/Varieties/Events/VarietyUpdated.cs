using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Varieties.Events;

public record VarietyUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }

  public Change<Genus>? Genus { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<GenderRatio>? GenderRatio { get; set; }

  public bool? CanChangeForm { get; set; }

  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
