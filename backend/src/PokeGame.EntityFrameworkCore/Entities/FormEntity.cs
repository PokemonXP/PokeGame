using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Events;
using PokeGame.Core.Forms.Models;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class FormEntity : AggregateEntity
{
  public int FormId { get; private set; }
  public Guid Id { get; private set; }

  public SpeciesEntity? Species { get; private set; }
  public int SpeciesId { get; private set; }
  public Guid SpeciesUid { get; private set; }

  public VarietyEntity? Variety { get; private set; }
  public int VarietyId { get; private set; }
  public Guid VarietyUid { get; private set; }
  public bool IsDefault { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public bool IsBattleOnly { get; private set; }
  public bool IsMega { get; private set; }

  public int Height { get; private set; }
  public int Weight { get; private set; }

  public PokemonType PrimaryType { get; private set; }
  public PokemonType? SecondaryType { get; private set; }

  public byte BaseHP { get; private set; }
  public byte BaseAttack { get; private set; }
  public byte BaseDefense { get; private set; }
  public byte BaseSpecialAttack { get; private set; }
  public byte BaseSpecialDefense { get; private set; }
  public byte BaseSpeed { get; private set; }

  public int YieldExperience { get; private set; }
  public int YieldHP { get; private set; }
  public int YieldAttack { get; private set; }
  public int YieldDefense { get; private set; }
  public int YieldSpecialAttack { get; private set; }
  public int YieldSpecialDefense { get; private set; }
  public int YieldSpeed { get; private set; }

  public string SpriteDefault { get; private set; } = string.Empty;
  public string SpriteShiny { get; private set; } = string.Empty;
  public string? SpriteAlternative { get; private set; }
  public string? SpriteAlternativeShiny { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<FormAbilityEntity> Abilities { get; private set; } = [];
  public List<PokemonEntity> Pokemon { get; private set; } = [];
  public List<EvolutionEntity> SourceEvolutions { get; private set; } = [];
  public List<EvolutionEntity> TargetEvolutions { get; private set; } = [];

  public FormEntity(VarietyEntity variety, FormCreated @event) : base(@event)
  {
    Id = new FormId(@event.StreamId).ToGuid();

    SpeciesEntity species = variety.Species ?? throw new ArgumentException("The species is required.", nameof(variety));
    Species = species;
    SpeciesId = species.SpeciesId;
    SpeciesUid = species.Id;

    Variety = variety;
    VarietyId = variety.VarietyId;
    VarietyUid = variety.Id;
    IsDefault = @event.IsDefault;

    UniqueName = @event.UniqueName.Value;

    IsBattleOnly = @event.IsBattleOnly;
    IsMega = @event.IsMega;

    Height = @event.Height.Value;
    Weight = @event.Weight.Value;

    SetTypes(@event.Types);
    SetBaseStatistics(@event.BaseStatistics);
    SetYield(@event.Yield);
    SetSprites(@event.Sprites);
  }

  private FormEntity() : base()
  {
  }

  public void SetAbility(AbilitySlot slot, AbilityEntity ability)
  {
    FormAbilityEntity? entity = Abilities.SingleOrDefault(a => a.Slot == slot);
    if (entity is null)
    {
      entity = new FormAbilityEntity(this, ability, slot);
      Abilities.Add(entity);
    }
    else
    {
      entity.SetAbility(ability);
    }
  }

  public void SetUniqueName(FormUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(FormUpdated @event)
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

    if (@event.IsBattleOnly.HasValue)
    {
      IsBattleOnly = @event.IsBattleOnly.Value;
    }
    if (@event.IsMega.HasValue)
    {
      IsMega = @event.IsMega.Value;
    }

    if (@event.Height is not null)
    {
      Height = @event.Height.Value;
    }
    if (@event.Weight is not null)
    {
      Weight = @event.Weight.Value;
    }

    if (@event.Types is not null)
    {
      SetTypes(@event.Types);
    }
    if (@event.BaseStatistics is not null)
    {
      SetBaseStatistics(@event.BaseStatistics);
    }
    if (@event.Yield is not null)
    {
      SetYield(@event.Yield);
    }
    if (@event.Sprites is not null)
    {
      SetSprites(@event.Sprites);
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

  public FormTypesModel GetTypes() => new(PrimaryType, SecondaryType);
  private void SetTypes(IFormTypes types)
  {
    PrimaryType = types.Primary;
    SecondaryType = types.Secondary;
  }

  public BaseStatisticsModel GetBaseStatistics() => new(BaseHP, BaseAttack, BaseDefense, BaseSpecialAttack, BaseSpecialDefense, BaseSpeed);
  private void SetBaseStatistics(IBaseStatistics @base)
  {
    BaseHP = @base.HP;
    BaseAttack = @base.Attack;
    BaseDefense = @base.Defense;
    BaseSpecialAttack = @base.SpecialAttack;
    BaseSpecialDefense = @base.SpecialDefense;
    BaseSpeed = @base.Speed;
  }

  public YieldModel GetYield() => new(YieldExperience, YieldHP, YieldAttack, YieldDefense, YieldSpecialAttack, YieldSpecialDefense, YieldSpeed);
  private void SetYield(IYield yield)
  {
    YieldExperience = yield.Experience;
    YieldHP = yield.HP;
    YieldAttack = yield.Attack;
    YieldDefense = yield.Defense;
    YieldSpecialAttack = yield.SpecialAttack;
    YieldSpecialDefense = yield.SpecialDefense;
    YieldSpeed = yield.Speed;
  }

  public SpritesModel GetSprites() => new(SpriteDefault, SpriteShiny, SpriteAlternative, SpriteAlternativeShiny);
  private void SetSprites(Sprites sprites)
  {
    SpriteDefault = sprites.Default.Value;
    SpriteShiny = sprites.Shiny.Value;
    SpriteAlternative = sprites.Alternative?.Value;
    SpriteAlternativeShiny = sprites.AlternativeShiny?.Value;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
