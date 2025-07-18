using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;
using PokeGame.Core.Varieties;

namespace PokeGame.Core;

public class UniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique name is already used.";

  public string EntityType
  {
    get => (string)Data[nameof(EntityType)]!;
    private set => Data[nameof(EntityType)] = value;
  }
  public Guid EntityId
  {
    get => (Guid)Data[nameof(EntityId)]!;
    private set => Data[nameof(EntityId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
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
      error.Data[nameof(EntityType)] = EntityType;
      error.Data[nameof(EntityId)] = EntityId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UniqueNameAlreadyUsedException(Ability ability, AbilityId conflictId)
    : this("Ability", ability.Id.ToGuid(), conflictId.ToGuid(), ability.UniqueName, nameof(ability.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Form form, FormId conflictId)
    : this("Form", form.Id.ToGuid(), conflictId.ToGuid(), form.UniqueName, nameof(form.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Move move, MoveId conflictId)
    : this("Move", move.Id.ToGuid(), conflictId.ToGuid(), move.UniqueName, nameof(move.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Pokemon2 pokemon, PokemonId conflictId)
    : this("Pokemon", pokemon.Id.ToGuid(), conflictId.ToGuid(), pokemon.UniqueName, nameof(pokemon.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Region region, RegionId conflictId)
    : this("Region", region.Id.ToGuid(), conflictId.ToGuid(), region.UniqueName, nameof(region.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(PokemonSpecies species, SpeciesId conflictId)
    : this("Species", species.Id.ToGuid(), conflictId.ToGuid(), species.UniqueName, nameof(species.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Trainer trainer, TrainerId conflictId)
    : this("Trainer", trainer.Id.ToGuid(), conflictId.ToGuid(), trainer.UniqueName, nameof(trainer.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Variety variety, VarietyId conflictId)
    : this("Variety", variety.Id.ToGuid(), conflictId.ToGuid(), variety.UniqueName, nameof(variety.UniqueName))
  {
  }
  private UniqueNameAlreadyUsedException(string entityType, Guid entityId, Guid conflictId, UniqueName uniqueName, string propertyName)
    : base(BuildMessage(entityType, entityId, conflictId, uniqueName, propertyName))
  {
    EntityType = entityType;
    EntityId = entityId;
    ConflictId = conflictId;
    UniqueName = uniqueName.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string entityType, Guid entityId, Guid conflictId, UniqueName uniqueName, string propertyName)
  {
    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(EntityType), entityType)
      .AddData(nameof(EntityId), entityId)
      .AddData(nameof(ConflictId), conflictId)
      .AddData(nameof(UniqueName), uniqueName)
      .AddData(nameof(PropertyName), propertyName)
      .Build();
  }
}
