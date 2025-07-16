using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Forms;

public class Form : AggregateRoot
{
  public new FormId Id => new(base.Id);

  public SpeciesId SpeciesId { get; private set; }
  public VarietyId VarietyId { get; private set; }
  public bool IsDefault { get; private set; }

  private readonly UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The form has not been initialized.");
  public DisplayName? DisplayName { get; private set; }
  public Description? Description { get; private set; }

  public bool IsBattleOnly { get; private set; }
  public bool IsMega { get; private set; }

  public int Height { get; private set; }
  public int Weight { get; private set; }

  private readonly Types? _types = null;
  public Types Types => _types ?? throw new InvalidOperationException("The form has not been initialized.");
  private readonly FormAbilities? _abilities = null;
  public FormAbilities Abilities => _abilities ?? throw new InvalidOperationException("The form has not been initialized.");
  private readonly BaseStatistics? _baseStatistics = null;
  public BaseStatistics BaseStatistics => _baseStatistics ?? throw new InvalidOperationException("The form has not been initialized.");

  public Url? Url { get; private set; }
  public Notes? Notes { get; private set; }

  public Form() : base()
  {
  }

  public Form(
    Variety variety,
    UniqueName uniqueName,
    Types types,
    FormAbilities abilities,
    BaseStatistics baseStatistics,
    bool isDefault = false,
    ActorId? actorId = null,
    FormId? formId = null) : base((formId ?? FormId.NewId()).StreamId)
  {
    SpeciesId = variety.SpeciesId;
    VarietyId = variety.Id;
    IsDefault = isDefault;

    _uniqueName = uniqueName;

    _types = types;
    _abilities = abilities;
    _baseStatistics = baseStatistics;
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
