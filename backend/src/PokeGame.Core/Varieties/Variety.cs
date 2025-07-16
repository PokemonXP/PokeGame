using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Species;

namespace PokeGame.Core.Varieties;

public class Variety : AggregateRoot
{
  public new VarietyId Id => new(base.Id);

  public SpeciesId SpeciesId { get; private set; }
  public bool IsDefault { get; private set; }

  private readonly UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The variety has not been initialized.");
  public DisplayName? DisplayName { get; set; }

  public Genus? Genus { get; set; }
  public Description? Description { get; set; }

  public GenderRatio? GenderRatio { get; private set; }
  public bool CanChangeForm { get; private set; }

  public Url? Url { get; set; }
  public Notes? Notes { get; set; }

  public Variety() : base()
  {
  }

  public Variety(
    PokemonSpecies species,
    UniqueName uniqueName,
    bool isDefault = false,
    GenderRatio? genderRatio = null,
    bool canChangeForm = false,
    ActorId? actorId = null,
    VarietyId? varietyId = null) : base((varietyId ?? VarietyId.NewId()).StreamId)
  {
    SpeciesId = species.Id;
    IsDefault = isDefault;

    _uniqueName = uniqueName;

    GenderRatio = genderRatio;
    CanChangeForm = canChangeForm;
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
