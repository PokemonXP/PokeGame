using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Regions;

namespace PokeGame.Core.Battles.Events;

public record BattleUpdated : DomainEvent
{
  public DisplayName? Name { get; set; }
  public Location? Location { get; set; }
  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
