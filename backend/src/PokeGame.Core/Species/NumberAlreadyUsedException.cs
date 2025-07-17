using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;
using PokeGame.Core.Regions;

namespace PokeGame.Core.Species;

public class NumberAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified Pokémon number is already used.";

  public Guid SpeciesId
  {
    get => (Guid)Data[nameof(SpeciesId)]!;
    private set => Data[nameof(SpeciesId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public int Number
  {
    get => (int)Data[nameof(Number)]!;
    private set => Data[nameof(Number)] = value;
  }
  public Guid? RegionId
  {
    get => (Guid?)Data[nameof(RegionId)];
    private set => Data[nameof(RegionId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(SpeciesId)] = SpeciesId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(Number)] = Number;
      error.Data[nameof(RegionId)] = RegionId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public NumberAlreadyUsedException(PokemonSpecies species, SpeciesId conflictId, RegionId? regionId = null)
    : base(BuildMessage(species, conflictId, regionId))
  {
    SpeciesId = species.Id.ToGuid();
    ConflictId = conflictId.ToGuid();
    Number = regionId.HasValue ? species.RegionalNumbers[regionId.Value].Value : species.Number.Value;
    RegionId = regionId?.ToGuid();
    PropertyName = regionId.HasValue ? nameof(species.RegionalNumbers) : nameof(species.Number);
  }

  private static string BuildMessage(PokemonSpecies species, SpeciesId conflictId, RegionId? regionId) => new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(SpeciesId), species.Id.ToGuid())
      .AddData(nameof(ConflictId), conflictId.ToGuid())
      .AddData(nameof(Number), regionId.HasValue ? species.RegionalNumbers[regionId.Value].Value : species.Number.Value)
      .AddData(nameof(RegionId), regionId?.ToGuid())
      .AddData(nameof(PropertyName), regionId.HasValue ? nameof(species.RegionalNumbers) : nameof(species.Number))
      .Build();
}
