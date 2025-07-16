using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Items;

public class Item : AggregateRoot
{
  public new ItemId Id => new(base.Id);

  private readonly UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The item has not been initialized yet.");
  public DisplayName? DisplayName { get; set; }
  public Description? Description { get; set; }

  public Url? Url { get; set; }
  public Notes? Notes { get; set; }

  public Item() : base()
  {
  }

  public Item(UniqueName uniqueName, ActorId? actorId = null, ItemId? itemId = null)
    : base((itemId ?? ItemId.NewId()).StreamId)
  {
    _uniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
