using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Species.Events;

public record SpeciesUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }

  public Friendship? BaseFriendship { get; set; }
  public CatchRate? CatchRate { get; set; }
  public GrowthRate? GrowthRate { get; set; }

  public EggGroups? EggGroups { get; set; }

  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
