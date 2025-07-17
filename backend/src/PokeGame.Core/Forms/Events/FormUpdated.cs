using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Forms.Events;

public record FormUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public bool? IsBattleOnly { get; set; }
  public bool? IsMega { get; set; }

  public Height? Height { get; set; }
  public Weight? Weight { get; set; }

  public FormTypes? Types { get; set; }
  public FormAbilities? Abilities { get; set; }
  public BaseStatistics? BaseStatistics { get; set; }
  public Yield? Yield { get; set; }
  public Sprites? Sprites { get; set; }

  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
