using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;
using PokeGame.Core;
using PokeGame.Core.Abilities.Models;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Regions.Models;
using PokeGame.Core.Species.Models;
using PokeGame.Core.Trainers.Models;
using PokeGame.Core.Varieties.Models;
using PokeGame.EntityFrameworkCore.Entities;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;
using AggregateModel = Krakenar.Contracts.Aggregate;

namespace PokeGame.EntityFrameworkCore;

internal class PokemonMapper
{
  private readonly Dictionary<ActorId, Actor> _actors;
  private readonly Actor _system = new();

  public PokemonMapper()
  {
    _actors = [];
  }

  public PokemonMapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors) : this()
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public AbilityModel ToAbility(AbilityEntity source)
  {
    AbilityModel destination = new()
    {
      Id = source.Id,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Url = source.Url,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  public FormModel ToForm(FormEntity source)
  {
    if (source.Variety is null)
    {
      throw new ArgumentException("The variety is required.", nameof(source));
    }

    FormModel destination = new()
    {
      Id = source.Id,
      Variety = ToVariety(source.Variety),
      IsDefault = source.IsDefault,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      IsBattleOnly = source.IsBattleOnly,
      IsMega = source.IsMega,
      Height = source.Height,
      Weight = source.Weight,
      Url = source.Url,
      Notes = source.Notes
    };

    destination.Types.Primary = source.PrimaryType;
    destination.Types.Secondary = source.SecondaryType;

    foreach (FormAbilityEntity entity in source.Abilities)
    {
      if (entity.Ability is null)
      {
        throw new ArgumentException("The ability is required.", nameof(source));
      }
      AbilityModel ability = ToAbility(entity.Ability);

      switch (entity.Slot)
      {
        case AbilitySlot.Primary:
          destination.Abilities.Primary = ability;
          break;
        case AbilitySlot.Secondary:
          destination.Abilities.Secondary = ability;
          break;
        case AbilitySlot.Hidden:
          destination.Abilities.Hidden = ability;
          break;
      }
    }

    destination.BaseStatistics.HP = (byte)source.HPBase;
    destination.BaseStatistics.Attack = (byte)source.AttackBase;
    destination.BaseStatistics.Defense = (byte)source.DefenseBase;
    destination.BaseStatistics.SpecialAttack = (byte)source.SpecialAttackBase;
    destination.BaseStatistics.SpecialDefense = (byte)source.SpecialDefenseBase;
    destination.BaseStatistics.Speed = (byte)source.SpeedBase;

    destination.Yield.Experience = source.ExperienceYield;
    destination.Yield.HP = source.HPYield;
    destination.Yield.Attack = source.AttackYield;
    destination.Yield.Defense = source.DefenseYield;
    destination.Yield.SpecialAttack = source.SpecialAttackYield;
    destination.Yield.SpecialDefense = source.SpecialDefenseYield;
    destination.Yield.Speed = source.SpeedYield;

    destination.Sprites.Default = source.DefaultSprite;
    destination.Sprites.DefaultShiny = source.DefaultSpriteShiny;
    destination.Sprites.Alternative = source.AlternativeSprite;
    destination.Sprites.AlternativeShiny = source.AlternativeSpriteShiny;

    MapAggregate(source, destination);

    return destination;
  }

  public ItemModel ToItem(ItemEntity source)
  {
    if (source.MoveId.HasValue && source.Move is null)
    {
      throw new ArgumentException("The move is required.", nameof(source));
    }

    ItemModel destination = new()
    {
      Id = source.Id,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Price = source.Price,
      Category = source.Category,
      BattleItem = source.GetBattleItem(),
      Berry = source.GetBerry(),
      Medicine = source.GetMedicine(),
      PokeBall = source.GetPokeBall(),
      Sprite = source.Sprite,
      Url = source.Url,
      Notes = source.Notes,
    };

    if (source.Move is not null)
    {
      destination.TechnicalMachine = new TechnicalMachineModel(ToMove(source.Move));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public MoveModel ToMove(MoveEntity source)
  {
    MoveModel destination = new()
    {
      Id = source.Id,
      Type = source.Type,
      Category = source.Category,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Accuracy = source.Accuracy,
      Power = source.Power,
      PowerPoints = source.PowerPoints,
      Url = source.Url,
      Notes = source.Notes
    };

    if (source.StatusCondition.HasValue)
    {
      destination.Status = new InflictedStatusModel(source.StatusCondition.Value, source.StatusChance);
    }
    if (source.VolatileConditions is not null)
    {
      destination.VolatileConditions.AddRange(PokemonSerializer.Instance.Deserialize<string[]>(source.VolatileConditions) ?? []);
    }

    destination.StatisticChanges.Attack = source.AttackChange;
    destination.StatisticChanges.Defense = source.DefenseChange;
    destination.StatisticChanges.SpecialAttack = source.SpecialAttackChange;
    destination.StatisticChanges.SpecialDefense = source.SpecialDefenseChange;
    destination.StatisticChanges.Speed = source.SpeedChange;
    destination.StatisticChanges.Accuracy = source.AccuracyChange;
    destination.StatisticChanges.Evasion = source.EvasionChange;
    destination.StatisticChanges.Critical = source.CriticalChange;

    MapAggregate(source, destination);

    return destination;
  }

  public PokemonModel ToPokemon(PokemonEntity source)
  {
    if (source.Form is null)
    {
      throw new ArgumentException("The form is required.", nameof(source));
    }
    if (source.HeldItemId.HasValue && source.HeldItem is null)
    {
      throw new ArgumentException("The held item is required.", nameof(source));
    }

    PokemonModel destination = new()
    {
      Id = source.Id,
      Form = ToForm(source.Form),
      UniqueName = source.UniqueName,
      Nickname = source.Nickname,
      Gender = source.Gender,
      TeraType = source.TeraType,
      Size = new PokemonSizeModel(source.Height, source.Weight),
      AbilitySlot = source.AbilitySlot,
      Nature = new PokemonNatureModel(PokemonNatures.Instance.Find(source.Nature)),
      GrowthRate = source.GrowthRate,
      Level = source.Level,
      Experience = source.Experience,
      MaximumExperience = source.MaximumExperience,
      ToNextLevel = source.ToNextLevel,
      Statistics = source.GetStatistics(),
      Vitality = source.Vitality,
      Stamina = source.Stamina,
      StatusCondition = source.StatusCondition,
      Friendship = source.Friendship,
      Sprite = source.Sprite,
      Url = source.Url,
      Notes = source.Notes
    };

    if (source.HeldItem is not null)
    {
      destination.HeldItem = ToItem(source.HeldItem);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public RegionModel ToRegion(RegionEntity source)
  {
    RegionModel destination = new()
    {
      Id = source.Id,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Url = source.Url,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  public SpeciesModel ToSpecies(SpeciesEntity source)
  {
    SpeciesModel destination = new()
    {
      Id = source.Id,
      Number = source.Number,
      Category = source.Category,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      BaseFriendship = source.BaseFriendship,
      CatchRate = source.CatchRate,
      GrowthRate = source.GrowthRate,
      Url = source.Url,
      Notes = source.Notes
    };

    foreach (RegionalNumberEntity regionalNumber in source.RegionalNumbers)
    {
      if (regionalNumber.Region is null)
      {
        throw new ArgumentException("The region is required.", nameof(source));
      }
      destination.RegionalNumbers.Add(new RegionalNumberModel(ToRegion(regionalNumber.Region), regionalNumber.Number));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public TrainerModel ToTrainer(TrainerEntity source)
  {
    TrainerModel destination = new()
    {
      Id = source.Id,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Gender = source.Gender,
      License = source.License,
      Money = source.Money,
      UserId = source.UserId,
      Sprite = source.Sprite,
      Url = source.Url,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  public VarietyModel ToVariety(VarietyEntity source)
  {
    if (source.Species is null)
    {
      throw new ArgumentException("The species is required.", nameof(source));
    }

    VarietyModel destination = new()
    {
      Id = source.Id,
      Species = ToSpecies(source.Species),
      IsDefault = source.IsDefault,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      CanChangeForm = source.CanChangeForm,
      GenderRatio = source.GenderRatio,
      Genus = source.Genus,
      Url = source.Url,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, AggregateModel destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(string? id) => TryFindActor(id) ?? _system;
  private Actor FindActor(ActorId? id) => TryFindActor(id) ?? _system;
  private Actor? TryFindActor(string? id) => string.IsNullOrWhiteSpace(id) ? null : TryFindActor(new ActorId(id));
  private Actor? TryFindActor(ActorId? id)
  {
    if (id.HasValue)
    {
      return _actors.TryGetValue(id.Value, out Actor? actor) ? actor : null;
    }
    return null;
  }
}
