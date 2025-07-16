using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Abilities;

public class Ability : AggregateRoot
{
  public new AbilityId Id => new(base.Id);

  private readonly UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The ability has not been initialized.");
  public DisplayName? DisplayName { get; }
  public Description? Description { get; }

  public Url? Url { get; }
  public Notes? Notes { get; }

  public Ability() : base()
  {
  }

  public Ability(UniqueName uniqueName, ActorId? actorId = null, AbilityId? abilityId = null)
    : base((abilityId ?? AbilityId.NewId()).StreamId)
  {
    _uniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
