using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Pokemons.Events;
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
  public PokemonNature Nature { get; private set; }

  public byte IndividualValueHp { get; private set; }
  public byte IndividualValueAttack { get; private set; }
  public byte IndividualValueDefense { get; private set; }
  public byte IndividualValueSpecialAttack { get; private set; }
  public byte IndividualValueSpecialDefense { get; private set; }
  public byte IndividualValueSpeed { get; private set; }

  public byte EffortValueHp { get; private set; }
  public byte EffortValueAttack { get; private set; }
  public byte EffortValueDefense { get; private set; }
  public byte EffortValueSpecialAttack { get; private set; }
  public byte EffortValueSpecialDefense { get; private set; }
  public byte EffortValueSpeed { get; private set; }

  public int Experience { get; private set; }
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
    Nature = @event.Nature;

    IndividualValueHp = @event.IndividualValues.HP;
    IndividualValueAttack = @event.IndividualValues.Attack;
    IndividualValueDefense = @event.IndividualValues.Defense;
    IndividualValueSpecialAttack = @event.IndividualValues.SpecialAttack;
    IndividualValueSpecialDefense = @event.IndividualValues.SpecialDefense;
    IndividualValueSpeed = @event.IndividualValues.Speed;

    EffortValueHp = @event.EffortValues.HP;
    EffortValueAttack = @event.EffortValues.Attack;
    EffortValueDefense = @event.EffortValues.Defense;
    EffortValueSpecialAttack = @event.EffortValues.SpecialAttack;
    EffortValueSpecialDefense = @event.EffortValues.SpecialDefense;
    EffortValueSpeed = @event.EffortValues.Speed;

    Experience = @event.Experience;
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
