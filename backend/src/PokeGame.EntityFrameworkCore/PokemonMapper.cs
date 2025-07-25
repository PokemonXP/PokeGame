using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Abilities.Models;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Inventory.Models;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;
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
      Types = source.GetTypes(),
      BaseStatistics = source.GetBaseStatistics(),
      Yield = source.GetYield(),
      Sprites = source.GetSprites(),
      Url = source.Url,
      Notes = source.Notes
    };

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

    MapAggregate(source, destination);

    return destination;
  }

  public InventoryItemModel ToInventoryItem(InventoryEntity source)
  {
    if (source.Item is null)
    {
      throw new ArgumentException("The item is required.", nameof(source));
    }

    return new InventoryItemModel(ToItem(source.Item), source.Quantity);
  }

  public ItemModel ToItem(ItemEntity source)
  {
    ItemModel destination = new()
    {
      Id = source.Id,
      Category = source.Category,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Price = source.Price,
      Sprite = source.Sprite,
      Url = source.Url,
      Notes = source.Notes,
    };

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

    MapAggregate(source, destination);

    return destination;
  }

  public PokemonModel ToPokemon(PokemonEntity source)
  {
    if (source.Form is null)
    {
      throw new ArgumentException("The form is required.", nameof(source));
    }

    PokemonModel destination = new()
    {
      Id = source.Id,
      Form = ToForm(source.Form),
      UniqueName = source.UniqueName,
      Nickname = source.Nickname,
      Gender = source.Gender,
      IsShiny = source.IsShiny,
      TeraType = source.TeraType,
      Size = new PokemonSizeModel(source.Height, source.Weight),
      AbilitySlot = source.AbilitySlot,
      Nature = new PokemonNatureModel(PokemonNatures.Instance.Find(source.Nature)),
      EggCycles = source.EggCycles,
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
      Characteristic = source.Characteristic,
      Sprite = source.Sprite,
      Url = source.Url,
      Notes = source.Notes
    };

    if (source.HeldItem is null && source.HeldItemId.HasValue)
    {
      throw new ArgumentException("The held item is required.", nameof(source));
    }
    else if (source.HeldItem is not null)
    {
      destination.HeldItem = ToItem(source.HeldItem);
    }

    foreach (PokemonMoveEntity entity in source.Moves)
    {
      if (entity.Move is null)
      {
        throw new ArgumentException("The moves are required.", nameof(source));
      }
      else if (entity.Item is null && entity.ItemId.HasValue)
      {
        throw new ArgumentException("The move item is required.", nameof(source));
      }

      PokemonMoveModel move = new()
      {
        Move = ToMove(entity.Move),
        Position = entity.Position,
        PowerPoints = new PowerPointsModel(entity.CurrentPowerPoints, entity.MaximumPowerPoints, entity.ReferencePowerPoints),
        IsMastered = entity.IsMastered,
        Level = entity.Level,
        Method = entity.Method,
        Notes = entity.Notes
      };
      if (entity.Item is not null)
      {
        move.Item = ToItem(entity.Item);
      }
      destination.Moves.Add(move);
    }

    if (source.OriginalTrainerId.HasValue && source.CurrentTrainerId.HasValue && source.PokeBallId.HasValue
      && source.OwnershipKind.HasValue && source.MetAtLevel.HasValue && source.MetLocation is not null
      && source.MetOn.HasValue && source.Position.HasValue)
    {
      if (source.OriginalTrainer is null)
      {
        throw new ArgumentException("The original trainer is required.", nameof(source));
      }
      if (source.CurrentTrainer is null)
      {
        throw new ArgumentException("The current trainer is required.", nameof(source));
      }
      if (source.PokeBall is null)
      {
        throw new ArgumentException("The Poké Ball is required.", nameof(source));
      }

      destination.Ownership = new OwnershipModel
      {
        OriginalTrainer = ToTrainer(source.OriginalTrainer),
        CurrentTrainer = ToTrainer(source.CurrentTrainer),
        PokeBall = ToItem(source.PokeBall),
        Kind = source.OwnershipKind.Value,
        Level = source.MetAtLevel.Value,
        Location = source.MetLocation,
        MetOn = source.MetOn.Value,
        Description = source.MetDescription,
        Position = source.Position.Value,
        Box = source.Box
      };
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
      EggCycles = source.EggCycles,
      EggGroups = source.GetEggGroups(),
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
      License = source.License,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Gender = source.Gender,
      Money = source.Money,
      UserId = source.UserUid,
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
      Genus = source.Genus,
      Description = source.Description,
      GenderRatio = source.GenderRatio,
      CanChangeForm = source.CanChangeForm,
      Url = source.Url,
      Notes = source.Notes
    };

    foreach (VarietyMoveEntity varietyMove in source.Moves)
    {
      if (varietyMove.Move is null)
      {
        throw new ArgumentException("The move is required.", nameof(source));
      }
      destination.Moves.Add(new VarietyMoveModel(ToMove(varietyMove.Move), varietyMove.Level));
    }

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
