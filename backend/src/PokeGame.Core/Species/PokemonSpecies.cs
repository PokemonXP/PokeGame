using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Species;

public class PokemonSpecies : AggregateRoot
{
  public new SpeciesId Id => new(base.Id);

  public int Number { get; private set; }
  public PokemonCategory Category { get; private set; }

  private readonly UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The species has not been initialized.");
  public DisplayName? DisplayName { get; set; }

  private readonly Friendship? _baseFriendship = null;
  public Friendship BaseFriendship => _baseFriendship ?? throw new InvalidOperationException("The species has not been initialized.");
  private readonly CatchRate? _catchRate = null;
  public CatchRate CatchRate => _catchRate ?? throw new InvalidOperationException("The species has not been initialized.");
  public GrowthRate GrowthRate { get; private set; }

  public Url? Url { get; set; }
  public Notes? Notes { get; set; }

  public PokemonSpecies() : base()
  {
  }

  public PokemonSpecies(
    int number,
    UniqueName uniqueName,
    CatchRate catchRate,
    PokemonCategory category = PokemonCategory.Standard,
    Friendship? baseFriendship = null,
    GrowthRate growthRate = GrowthRate.MediumFast,
    ActorId? actorId = null,
    SpeciesId? speciesId = null) : base((speciesId ?? SpeciesId.NewId()).StreamId)
  {
    if (!Enum.IsDefined(category))
    {
      throw new ArgumentOutOfRangeException(nameof(category));
    }
    if (!Enum.IsDefined(growthRate))
    {
      throw new ArgumentOutOfRangeException(nameof(growthRate));
    }

    Number = number;
    Category = category;

    _uniqueName = uniqueName;

    _baseFriendship = baseFriendship ?? new Friendship(0);
    _catchRate = catchRate;
    GrowthRate = growthRate;
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
