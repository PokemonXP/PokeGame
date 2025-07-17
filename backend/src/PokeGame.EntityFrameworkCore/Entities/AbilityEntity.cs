using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core.Abilities;
using PokeGame.Core.Abilities.Events;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class AbilityEntity : AggregateEntity
{
  public int AbilityId { get; private set; }
  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<FormAbilityEntity> Forms { get; private set; } = [];

  public AbilityEntity(AbilityCreated @event) : base(@event)
  {
    Id = new AbilityId(@event.StreamId).ToGuid();

    UniqueName = @event.UniqueName.Value;
  }

  private AbilityEntity() : base()
  {
  }

  public void SetUniqueName(AbilityUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(AbilityUpdated @event)
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
