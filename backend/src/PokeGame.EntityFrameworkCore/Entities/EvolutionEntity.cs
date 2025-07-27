using PokeGame.Core;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Evolutions.Events;
using PokeGame.Core.Pokemon;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class EvolutionEntity : AggregateEntity
{
  public int EvolutionId { get; private set; }
  public Guid Id { get; private set; }

  public FormEntity? Source { get; private set; }
  public int SourceId { get; private set; }
  public Guid SourceUid { get; private set; }

  public FormEntity? Target { get; private set; }
  public int TargetId { get; private set; }
  public Guid TargetUid { get; private set; }

  public EvolutionTrigger Trigger { get; private set; }

  public ItemEntity? Item { get; private set; }
  public int? ItemId { get; private set; }
  public Guid? ItemUid { get; private set; }

  public int Level { get; private set; }
  public bool Friendship { get; private set; }
  public PokemonGender? Gender { get; private set; }
  public string? Location { get; private set; }
  public TimeOfDay? TimeOfDay { get; private set; }

  public ItemEntity? HeldItem { get; private set; }
  public int? HeldItemId { get; private set; }
  public Guid? HeldItemUid { get; private set; }

  public MoveEntity? KnownMove { get; private set; }
  public int? KnownMoveId { get; private set; }
  public Guid? KnownMoveUid { get; private set; }

  public EvolutionEntity(FormEntity source, FormEntity target, ItemEntity? item, EvolutionCreated @event) : base(@event)
  {
    Source = source;
    SourceId = source.FormId;
    SourceUid = source.Id;

    Target = target;
    TargetId = target.FormId;
    TargetUid = target.Id;

    Trigger = @event.Trigger;

    Item = item;
    ItemId = item?.ItemId;
    ItemUid = item?.Id;
  }

  private EvolutionEntity() : base()
  {
  }

  public void SetHeldItem(ItemEntity? item)
  {
    HeldItem = item;
    HeldItemId = item?.ItemId;
    HeldItemUid = item?.Id;
  }

  public void SetKnownMove(MoveEntity? move)
  {
    KnownMove = move;
    KnownMoveId = move?.MoveId;
    KnownMoveUid = move?.Id;
  }

  public void Update(EvolutionUpdated @event)
  {
    base.Update(@event);

    if (@event.Level is not null)
    {
      Level = @event.Level.Value?.Value ?? 0;
    }
    if (@event.Friendship.HasValue)
    {
      Friendship = @event.Friendship.Value;
    }
    if (@event.Gender is not null)
    {
      Gender = @event.Gender.Value;
    }
    if (@event.Location is not null)
    {
      Location = @event.Location.Value?.Value;
    }
    if (@event.TimeOfDay is not null)
    {
      TimeOfDay = @event.TimeOfDay.Value;
    }
  }
}
