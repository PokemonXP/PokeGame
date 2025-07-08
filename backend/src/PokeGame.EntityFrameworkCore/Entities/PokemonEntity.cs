using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Species;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class PokemonEntity : AggregateEntity
{
  public int PokemonId { get; private set; }
  public Guid Id { get; private set; }

  public SpeciesEntity? Species { get; private set; }
  public int SpeciesId { get; private set; }
  public Guid SpeciesUid { get; private set; }

  public VarietyEntity? Variety { get; private set; }
  public int VarietyId { get; private set; }
  public Guid VarietyUid { get; private set; }

  public FormEntity? Form { get; private set; }
  public int FormId { get; private set; }
  public Guid FormUid { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? Nickname { get; private set; }
  public PokemonGender? Gender { get; private set; }

  public PokemonType TeraType { get; private set; }
  public byte Height { get; private set; }
  public byte Weight { get; private set; }
  public AbilitySlot AbilitySlot { get; private set; }
  public string Nature { get; private set; } = string.Empty;

  public GrowthRate GrowthRate { get; private set; }
  public int Experience { get; private set; }
  public int Level { get; private set; }
  public int MaximumExperience { get; private set; }
  public int ToNextLevel { get; private set; }

  public string? Statistics { get; private set; }
  public int Vitality { get; private set; }
  public int Stamina { get; private set; }

  public byte Friendship { get; private set; }

  public string? Sprite { get; private set; }
  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public PokemonEntity(FormEntity form, PokemonCreated @event) : base(@event)
  {
    VarietyEntity variety = form.Variety ?? throw new ArgumentException("The variety is required.", nameof(form));
    SpeciesEntity species = variety.Species ?? throw new ArgumentException("The species is required.", nameof(form));

    Id = new PokemonId(@event.StreamId).ToGuid();

    Species = species;
    SpeciesId = species.SpeciesId;
    SpeciesUid = species.Id;

    Variety = variety;
    VarietyId = variety.VarietyId;
    VarietyUid = variety.Id;

    Form = form;
    FormId = form.FormId;
    FormUid = form.Id;

    UniqueName = @event.UniqueName.Value;
    Gender = @event.Gender;

    TeraType = @event.TeraType;
    Height = @event.Size.Height;
    Weight = @event.Size.Weight;
    AbilitySlot = @event.AbilitySlot;
    Nature = @event.Nature.Name;

    GrowthRate = @event.GrowthRate;
    Experience = @event.Experience;
    Level = ExperienceTable.Instance.GetLevel(GrowthRate, Experience);
    MaximumExperience = ExperienceTable.Instance.GetMaximumExperience(GrowthRate, Level);
    ToNextLevel = Math.Max(MaximumExperience - Experience, 0);

    Statistics = null; // TODO(fpion): implement
    Vitality = @event.Vitality;
    Stamina = @event.Stamina;

    Friendship = @event.Friendship;
  }

  private PokemonEntity()
  {
  }

  public void SetNickname(PokemonNicknamed @event)
  {
    Update(@event);

    Nickname = @event.Nickname?.Value;
  }

  public void SetUniqueName(PokemonUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(PokemonUpdated @event)
  {
    base.Update(@event);

    if (@event.Gender is not null)
    {
      Gender = @event.Gender.Value;
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

  public override string ToString() => $"{Nickname ?? UniqueName}";
}
