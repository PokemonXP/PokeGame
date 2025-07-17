using Krakenar.Core;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace PokeGame.Core.Trainers.Events;

public record TrainerUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public TrainerGender? Gender { get; set; }
  public Money? Money { get; set; }

  public Change<UserId?>? UserId { get; set; }

  public Change<Url>? Sprite { get; set; }
  public Change<Url>? Url { get; set; }
  public Change<Notes>? Notes { get; set; }
}
