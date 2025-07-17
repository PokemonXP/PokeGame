using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Events;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class MoveEntity : AggregateEntity
{
  public int MoveId { get; private set; }
  public Guid Id { get; private set; }

  public PokemonType Type { get; private set; }
  public MoveCategory Category { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public byte? Accuracy { get; private set; }
  public byte? Power { get; private set; }
  public byte PowerPoints { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<PokemonMoveEntity> Pokemon { get; private set; } = [];
  public List<ItemEntity> TechnicalMachines { get; private set; } = [];

  public MoveEntity(MoveCreated @event) : base(@event)
  {
    Id = new MoveId(@event.StreamId).ToGuid();

    Type = @event.Type;
    Category = @event.Category;

    UniqueName = @event.UniqueName.Value;

    Accuracy = @event.Accuracy?.Value;
    Power = @event.Power?.Value;
    PowerPoints = @event.PowerPoints.Value;
  }

  private MoveEntity() : base()
  {
  }

  public void SetUniqueName(MoveUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(MoveUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Accuracy is not null)
    {
      Accuracy = @event.Accuracy.Value?.Value;
    }
    if (@event.Power is not null)
    {
      Power = @event.Power.Value?.Value;
    }
    if (@event.PowerPoints is not null)
    {
      PowerPoints = @event.PowerPoints.Value;
    }

    if (@event.Url is not null)
    {
      Url = @event.Url.Value?.Value;
    }
    if (@event.Notes is not null)
    {
      Notes = @event.Notes.Value?.Value;
    }
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
