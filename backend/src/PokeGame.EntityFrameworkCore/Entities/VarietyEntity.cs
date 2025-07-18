using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core.Varieties;
using PokeGame.Core.Varieties.Events;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class VarietyEntity : AggregateEntity
{
  public int VarietyId { get; private set; }
  public Guid Id { get; private set; }

  public SpeciesEntity? Species { get; set; }
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

  public string? Genus { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public int? GenderRatio { get; private set; }

  public bool CanChangeForm { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<FormEntity> Forms { get; private set; } = [];
  public List<VarietyMoveEntity> Moves { get; private set; } = [];
  public List<PokemonEntity> Pokemon { get; private set; } = [];

  public VarietyEntity(SpeciesEntity species, VarietyCreated @event) : base(@event)
  {
    Id = new VarietyId(@event.StreamId).ToGuid();

    Species = species;
    SpeciesId = species.SpeciesId;
    SpeciesUid = species.Id;

    IsDefault = @event.IsDefault;

    UniqueName = @event.UniqueName.Value;

    GenderRatio = @event.GenderRatio?.Value;

    CanChangeForm = @event.CanChangeForm;
  }

  private VarietyEntity() : base()
  {
  }

  public VarietyMoveEntity? RemoveMove(VarietyMoveRemoved @event)
  {
    Update(@event);

    return Moves.SingleOrDefault(m => m.Move?.StreamId == @event.MoveId.Value);
  }

  public void SetEvolutionMove(MoveEntity move, VarietyEvolutionMoveChanged @event)
  {
    Update(@event);

    VarietyMoveEntity? varietyMove = Moves.SingleOrDefault(m => m.Move?.StreamId == @event.MoveId.Value);
    if (varietyMove is null)
    {
      varietyMove = new VarietyMoveEntity(this, move, @event);
      Moves.Add(varietyMove);
    }
    else
    {
      varietyMove.Update(@event);
    }
  }

  public void SetLevelMove(MoveEntity move, VarietyLevelMoveChanged @event)
  {
    Update(@event);

    VarietyMoveEntity? varietyMove = Moves.SingleOrDefault(m => m.Move?.StreamId == @event.MoveId.Value);
    if (varietyMove is null)
    {
      varietyMove = new VarietyMoveEntity(this, move, @event);
      Moves.Add(varietyMove);
    }
    else
    {
      varietyMove.Update(@event);
    }
  }

  public void SetUniqueName(VarietyUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(VarietyUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }

    if (@event.Genus is not null)
    {
      Genus = @event.Genus.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.GenderRatio is not null)
    {
      GenderRatio = @event.GenderRatio.Value?.Value;
    }

    if (@event.CanChangeForm.HasValue)
    {
      CanChangeForm = @event.CanChangeForm.Value;
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
