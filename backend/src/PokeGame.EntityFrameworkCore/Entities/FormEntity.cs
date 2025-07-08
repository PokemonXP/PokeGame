using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.Infrastructure.Data;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class FormEntity : AggregateEntity
{
  public int FormId { get; private set; }
  public Guid Id { get; private set; }

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

  public int HPBase { get; private set; }
  public int AttackBase { get; private set; }
  public int DefenseBase { get; private set; }
  public int SpecialAttackBase { get; private set; }
  public int SpecialDefenseBase { get; private set; }
  public int SpeedBase { get; private set; }

  public int ExperienceYield { get; private set; }
  public int HPYield { get; private set; }
  public int AttackYield { get; private set; }
  public int DefenseYield { get; private set; }
  public int SpecialAttackYield { get; private set; }
  public int SpecialDefenseYield { get; private set; }
  public int SpeedYield { get; private set; }

  public string DefaultSprite { get; private set; } = string.Empty;
  public string DefaultSpriteShiny { get; private set; } = string.Empty;
  public string? AlternativeSprite { get; private set; }
  public string? AlternativeSpriteShiny { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<FormAbilityEntity> Abilities { get; private set; } = [];
  public List<PokemonEntity> Pokemon { get; private set; } = [];

  public FormEntity(VarietyEntity variety, FormPublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Variety = variety;
    VarietyId = variety.VarietyId;
    VarietyUid = variety.Id;

    Update(published);
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

  public void Update(FormPublished published)
  {
    ContentLocale invariant = published.Invariant;
    ContentLocale locale = published.Locale;

    Update(published.Event);

    IsDefault = invariant.FindBooleanValue(Forms.IsDefault);

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    IsBattleOnly = invariant.FindBooleanValue(Forms.IsBattleOnly);
    IsMega = invariant.FindBooleanValue(Forms.IsMega);

    Height = (int)invariant.FindNumberValue(Forms.Height);
    Weight = (int)invariant.FindNumberValue(Forms.Weight);

    PrimaryType = PokemonConverter.Instance.ToType(invariant.FindSelectValue(Forms.PrimaryType).Single());
    string? secondaryType = invariant.TryGetSelectValue(Forms.SecondaryType)?.Single();
    SecondaryType = secondaryType is null ? null : PokemonConverter.Instance.ToType(secondaryType);

    HPBase = (int)invariant.FindNumberValue(Forms.HPBase);
    AttackBase = (int)invariant.FindNumberValue(Forms.AttackBase);
    DefenseBase = (int)invariant.FindNumberValue(Forms.DefenseBase);
    SpecialAttackBase = (int)invariant.FindNumberValue(Forms.SpecialAttackBase);
    SpecialDefenseBase = (int)invariant.FindNumberValue(Forms.SpecialDefenseBase);
    SpeedBase = (int)invariant.FindNumberValue(Forms.SpeedBase);

    ExperienceYield = (int)invariant.FindNumberValue(Forms.ExperienceYield);
    HPYield = (int)invariant.GetNumberValue(Forms.HPYield);
    AttackYield = (int)invariant.GetNumberValue(Forms.AttackYield);
    DefenseYield = (int)invariant.GetNumberValue(Forms.DefenseYield);
    SpecialAttackYield = (int)invariant.GetNumberValue(Forms.SpecialAttackYield);
    SpecialDefenseYield = (int)invariant.GetNumberValue(Forms.SpecialDefenseYield);
    SpeedYield = (int)invariant.GetNumberValue(Forms.SpeedYield);

    DefaultSprite = invariant.FindStringValue(Forms.DefaultSprite);
    DefaultSpriteShiny = invariant.FindStringValue(Forms.DefaultSpriteShiny);
    AlternativeSprite = invariant.TryGetStringValue(Forms.AlternativeSprite);
    AlternativeSpriteShiny = invariant.TryGetStringValue(Forms.AlternativeSpriteShiny);

    Url = locale.TryGetStringValue(Forms.Url);
    Notes = locale.TryGetStringValue(Forms.Notes);
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
