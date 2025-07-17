using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core.Species;
using PokeGame.Core.Species.Events;
using PokeGame.Core.Species.Models;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class SpeciesEntity : AggregateEntity
{
  public int SpeciesId { get; private set; }
  public Guid Id { get; private set; }

  public int Number { get; private set; }
  public PokemonCategory Category { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }

  public byte BaseFriendship { get; private set; }
  public byte CatchRate { get; private set; }
  public GrowthRate GrowthRate { get; private set; }

  public byte EggCycles { get; private set; }
  public EggGroup PrimaryEggGroup { get; private set; }
  public EggGroup? SecondaryEggGroup { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<PokemonEntity> Pokemon { get; private set; } = [];
  public List<RegionalNumberEntity> RegionalNumbers { get; private set; } = [];
  public List<VarietyEntity> Varieties { get; private set; } = [];

  public SpeciesEntity(SpeciesCreated @event) : base(@event)
  {
    Id = new SpeciesId(@event.StreamId).ToGuid();

    Number = @event.Number.Value;
    Category = @event.Category;

    UniqueName = @event.UniqueName.Value;

    BaseFriendship = @event.BaseFriendship.Value;
    CatchRate = @event.CatchRate.Value;
    GrowthRate = @event.GrowthRate;

    EggCycles = @event.EggCycles.Value;
    SetEggGroups(@event.EggGroups);
  }

  private SpeciesEntity() : base()
  {
  }

  public RegionalNumberEntity? SetRegionalNumber(RegionEntity? region, SpeciesRegionalNumberChanged @event)
  {
    Update(@event);

    RegionalNumberEntity? regionalNumber = RegionalNumbers.SingleOrDefault(x => x.Region?.StreamId == @event.RegionId.Value);
    if (@event.Number is not null)
    {
      if (regionalNumber is null)
      {
        ArgumentNullException.ThrowIfNull(region, nameof(region));
        regionalNumber = new RegionalNumberEntity(this, region, @event);
        RegionalNumbers.Add(regionalNumber);
      }
      else
      {
        regionalNumber.Update(@event);
      }
    }

    return regionalNumber;
  }

  public void SetUniqueName(SpeciesUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(SpeciesUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }

    if (@event.BaseFriendship is not null)
    {
      BaseFriendship = @event.BaseFriendship.Value;
    }
    if (@event.CatchRate is not null)
    {
      CatchRate = @event.CatchRate.Value;
    }
    if (@event.GrowthRate.HasValue)
    {
      GrowthRate = @event.GrowthRate.Value;
    }

    if (@event.EggCycles is not null)
    {
      EggCycles = @event.EggCycles.Value;
    }
    if (@event.EggGroups is not null)
    {
      SetEggGroups(@event.EggGroups);
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

  public EggGroupsModel GetEggGroups() => new(PrimaryEggGroup, SecondaryEggGroup);
  private void SetEggGroups(IEggGroups eggGroups)
  {
    PrimaryEggGroup = eggGroups.Primary;
    SecondaryEggGroup = eggGroups.Secondary;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
