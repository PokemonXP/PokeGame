using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core;
using PokeGame.Core.Trainers;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.Infrastructure.Data;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class TrainerEntity : AggregateEntity
{
  public int TrainerId { get; private set; }
  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public TrainerGender Gender { get; private set; }
  public string License { get; private set; } = string.Empty;
  public string LicenseNormalized
  {
    get => Helper.Normalize(License);
    private set { }
  }
  public int Money { get; private set; }

  public Guid? UserId { get; private set; }

  public string? Sprite { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<PokemonEntity> CurrentPokemon { get; private set; } = [];
  public List<PokemonEntity> OriginalPokemon { get; private set; } = [];

  public TrainerEntity(TrainerPublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Update(published);
  }

  private TrainerEntity() : base()
  {
  }

  public void Update(TrainerPublished published)
  {
    ContentLocale invariant = published.Invariant;
    ContentLocale locale = published.Locale;

    Update(published.Event);

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    Gender = PokemonConverter.Instance.ToTrainerGender(invariant.FindSelectValue(Trainers.Gender).Single());
    License = invariant.FindStringValue(Trainers.License);
    Money = (int)invariant.FindNumberValue(Trainers.Money);

    string? userId = invariant.TryGetStringValue(Trainers.UserId);
    UserId = userId is null ? null : Guid.Parse(userId);

    Sprite = invariant.FindStringValue(Trainers.Sprite);

    Url = locale.TryGetStringValue(Trainers.Url);
    Notes = locale.TryGetStringValue(Trainers.Notes);
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
