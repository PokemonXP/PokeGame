using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core.Species;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.Infrastructure;
using PokeGame.Infrastructure.Data;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class SpeciesEntity : AggregateEntity
{
  public int SpeciesId { get; private set; }
  public Guid Id { get; private set; }

  public int Number { get; set; }
  public PokemonCategory Category { get; set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }

  public int BaseFriendship { get; set; }
  public int CatchRate { get; set; }
  public GrowthRate GrowthRate { get; set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<RegionalNumberEntity> RegionalNumbers { get; private set; } = [];
  public List<VarietyEntity> Varieties { get; private set; } = [];

  public SpeciesEntity(SpeciesPublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Update(published);
  }

  private SpeciesEntity() : base()
  {
  }

  public void SetRegionalNumber(RegionEntity region, int number)
  {
    RegionalNumberEntity? regionalNumber = RegionalNumbers.SingleOrDefault(x => x.RegionUid == region.Id);
    if (regionalNumber is null)
    {
      regionalNumber = new RegionalNumberEntity(this, region, number);
      RegionalNumbers.Add(regionalNumber);
    }
    else
    {
      regionalNumber.Number = number;
    }
  }

  public void Update(SpeciesPublished published)
  {
    var invariant = published.Invariant;
    ContentLocale locale = published.Locale;

    Update(published.Event);

    Number = (int)invariant.FindNumberValue(Species.Number);
    Category = Enum.Parse<PokemonCategory>(invariant.FindSelectValue(Species.Category).Single().Capitalize());

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;

    BaseFriendship = (int)invariant.FindNumberValue(Species.BaseFriendship);
    CatchRate = (int)invariant.FindNumberValue(Species.CatchRate);
    GrowthRate = ParseGrowthRate(invariant.FindSelectValue(Species.GrowthRate).Single());

    Url = locale.TryGetStringValue(Species.Url);
    Notes = locale.TryGetStringValue(Species.Notes);
  }

  private static GrowthRate ParseGrowthRate(string value)
  {
    return value switch
    {
      "slow-then-very-fast" => GrowthRate.Erratic,
      "fast" => GrowthRate.Fast,
      "fast-then-very-slow" => GrowthRate.Fluctuating,
      "medium" => GrowthRate.MediumFast,
      "medium-slow" => GrowthRate.MediumSlow,
      "slow" => GrowthRate.Slow,
      _ => throw new ArgumentException($"The growth rate '{value}' is not valid.", nameof(value)),
    };
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
