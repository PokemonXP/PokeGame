using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.Infrastructure.Data;
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

  public AbilityEntity(AbilityPublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Update(published);
  }

  private AbilityEntity() : base()
  {
  }

  public void Update(AbilityPublished published)
  {
    ContentLocale locale = published.Locale;

    Update(published.Event);

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    Url = locale.TryGetStringValue(Abilities.Url);
    Notes = locale.TryGetStringValue(Abilities.Notes);
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
