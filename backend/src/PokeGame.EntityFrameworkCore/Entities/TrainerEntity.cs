using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Events;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class TrainerEntity : AggregateEntity
{
  public int TrainerId { get; private set; }
  public Guid Id { get; private set; }

  public string License { get; private set; } = string.Empty;
  public string LicenseNormalized
  {
    get => Helper.Normalize(License);
    private set { }
  }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public TrainerGender Gender { get; private set; }
  public int Money { get; private set; }
  public int PartySize { get; private set; }

  public string? UserId { get; private set; }
  public Guid? UserUid { get; private set; }

  public string? Sprite { get; private set; }
  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<PokemonEntity> CurrentPokemon { get; private set; } = [];
  public List<PokemonEntity> OriginalPokemon { get; private set; } = [];
  public List<InventoryEntity> Inventory { get; private set; } = [];

  public TrainerEntity(TrainerCreated @event) : base(@event)
  {
    Id = new TrainerId(@event.StreamId).ToGuid();

    License = @event.License.Value;

    UniqueName = @event.UniqueName.Value;

    Gender = @event.Gender;
    Money = @event.Money.Value;
  }

  private TrainerEntity() : base()
  {
  }

  public void SetUniqueName(TrainerUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void SetUser(TrainerUserChanged @event)
  {
    Update(@event);

    UserId = @event.UserId?.Value;
    UserUid = @event.UserId?.EntityId;
  }

  public void Update(TrainerUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Gender.HasValue)
    {
      Gender = @event.Gender.Value;
    }
    if (@event.Money is not null)
    {
      Money = @event.Money.Value;
    }

    if (@event.Sprite is not null)
    {
      Sprite = @event.Sprite.Value?.Value;
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
