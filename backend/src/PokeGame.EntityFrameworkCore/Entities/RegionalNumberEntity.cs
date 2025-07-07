namespace PokeGame.EntityFrameworkCore.Entities;

internal class RegionalNumberEntity
{
  public int RegionalNumberId { get; private set; }

  public SpeciesEntity? Species { get; private set; }
  public int SpeciesId { get; private set; }
  public Guid SpeciesUid { get; private set; }

  public RegionEntity? Region { get; private set; }
  public int RegionId { get; private set; }
  public Guid RegionUid { get; private set; }

  public int Number { get; set; }

  public RegionalNumberEntity(SpeciesEntity species, RegionEntity region, int number)
  {
    Species = species;
    SpeciesId = species.SpeciesId;
    SpeciesUid = species.Id;

    Region = region;
    RegionId = region.RegionId;
    RegionUid = region.Id;

    Number = number;
  }

  private RegionalNumberEntity()
  {
  }

  public override bool Equals(object? obj) => obj is RegionalNumberEntity entity && entity.RegionalNumberId == RegionalNumberId;
  public override int GetHashCode() => RegionalNumberId.GetHashCode();
  public override string ToString() => $"{GetType()} (RegionalNumberId={RegionalNumberId})";
}
