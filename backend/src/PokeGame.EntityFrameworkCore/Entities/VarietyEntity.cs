using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.Infrastructure.Data;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class VarietyEntity : AggregateEntity
{
  public int VarietyId { get; private set; }
  public Guid Id { get; private set; }

  public SpeciesEntity? Species { get; private set; }
  public int SpeciesId { get; private set; }
  public Guid SpeciesUid { get; private set; }
  public bool IsDefault { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public bool CanChangeForm { get; private set; }
  public int? GenderRatio { get; private set; }

  public string Genus { get; private set; } = string.Empty;

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<FormEntity> Forms { get; private set; } = [];

  public VarietyEntity(SpeciesEntity species, VarietyPublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Species = species;
    SpeciesId = species.SpeciesId;
    SpeciesUid = species.Id;

    Update(published);
  }

  private VarietyEntity() : base()
  {
  }

  public void Update(VarietyPublished published)
  {
    ContentLocale invariant = published.Invariant;
    ContentLocale locale = published.Locale;

    Update(published.Event);

    IsDefault = invariant.FindBooleanValue(Varieties.IsDefault);

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    CanChangeForm = invariant.FindBooleanValue(Varieties.CanChangeForm);
    GenderRatio = (int)invariant.FindNumberValue(Varieties.GenderRatio);

    Genus = locale.FindStringValue(Varieties.Genus);

    Url = locale.TryGetStringValue(Varieties.Url);
    Notes = locale.TryGetStringValue(Varieties.Notes);
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
