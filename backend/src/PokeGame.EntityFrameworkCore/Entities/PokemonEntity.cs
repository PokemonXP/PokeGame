using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar;
using PokeGame.Core;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Events;
using PokeGame.Core.Pokemon.Models;
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

  public FormEntity? Form { get; set; }
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
  public bool IsShiny { get; private set; }

  public PokemonType TeraType { get; private set; }
  public byte Height { get; private set; }
  public byte Weight { get; private set; }
  public AbilitySlot AbilitySlot { get; private set; }
  public string Nature { get; private set; } = string.Empty;

  public byte EggCycles { get; private set; }
  public GrowthRate GrowthRate { get; private set; }
  public int Level { get; private set; }
  public int Experience { get; private set; }
  public int MaximumExperience { get; private set; }
  public int ToNextLevel { get; private set; }

  public string Statistics { get; private set; } = string.Empty;
  public int Vitality { get; private set; }
  public int Stamina { get; private set; }
  public StatusCondition? StatusCondition { get; private set; }
  public byte Friendship { get; private set; }

  public string Characteristic { get; private set; } = string.Empty;

  public ItemEntity? HeldItem { get; private set; }
  public int? HeldItemId { get; private set; }
  public Guid? HeldItemUid { get; private set; }

  public TrainerEntity? OriginalTrainer { get; private set; }
  public int? OriginalTrainerId { get; private set; }
  public Guid? OriginalTrainerUid { get; private set; }

  public TrainerEntity? CurrentTrainer { get; private set; }
  public int? CurrentTrainerId { get; private set; }
  public Guid? CurrentTrainerUid { get; private set; }

  public ItemEntity? PokeBall { get; private set; }
  public int? PokeBallId { get; private set; }
  public Guid? PokeBallUid { get; private set; }

  public OwnershipKind? OwnershipKind { get; private set; }
  public int? MetAtLevel { get; private set; }
  public string? MetLocation { get; private set; }
  public DateTime? MetOn { get; private set; }
  public string? MetDescription { get; private set; }
  public int? Position { get; private set; }
  public int? Box { get; private set; }

  public string? Sprite { get; private set; }
  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<PokemonMoveEntity> Moves { get; private set; } = [];

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
    IsShiny = @event.IsShiny;

    TeraType = @event.TeraType;
    Height = @event.Size.Height;
    Weight = @event.Size.Weight;
    AbilitySlot = @event.AbilitySlot;
    Nature = @event.Nature.Name;

    EggCycles = @event.EggCycles?.Value ?? 0;
    GrowthRate = @event.GrowthRate;
    Experience = @event.Experience;
    Level = ExperienceTable.Instance.GetLevel(GrowthRate, Experience);
    MaximumExperience = ExperienceTable.Instance.GetMaximumExperience(GrowthRate, Level);
    ToNextLevel = Math.Max(MaximumExperience - Experience, 0);

    SetStatistics(@event.BaseStatistics, @event.IndividualValues, @event.EffortValues);
    Vitality = @event.Vitality;
    Stamina = @event.Stamina;
    Friendship = @event.Friendship.Value;

    Characteristic = PokemonCharacteristics.Instance.Find(@event.IndividualValues, @event.Size).Text;
  }

  private PokemonEntity()
  {
  }

  public void Catch(TrainerEntity trainer, ItemEntity pokeBall, PokemonCaught @event)
  {
    Update(@event);

    SetOwnership(trainer, pokeBall, Core.Pokemon.OwnershipKind.Caught, @event, @event.OccurredOn);
  }

  public void HoldItem(ItemEntity item, PokemonItemHeld @event)
  {
    Update(@event);

    HeldItem = item;
    HeldItemId = item.ItemId;
    HeldItemUid = item.Id;
  }

  public void LearnMove(MoveEntity move, PokemonMoveLearned @event)
  {
    Update(@event);

    RemoveMove(@event.Position);
    Moves.Add(new PokemonMoveEntity(this, move, @event));
  }

  public void Receive(TrainerEntity trainer, ItemEntity pokeBall, PokemonReceived @event)
  {
    Update(@event);

    SetOwnership(trainer, pokeBall, Core.Pokemon.OwnershipKind.Received, @event, @event.OccurredOn);
  }

  public void Release(PokemonReleased @event)
  {
    Update(@event);

    SetOriginalTrainer(null);
    SetCurrentTrainer(null);
    SetPokeBall(null);

    OwnershipKind = null;
    MetAtLevel = null;
    MetLocation = null;
    MetOn = null;
    MetDescription = null;

    SetSlot(null);
  }

  public bool RememberMove(PokemonMoveRemembered @event)
  {
    Update(@event);

    RemoveMove(@event.Position);

    PokemonMoveEntity? move = Moves.SingleOrDefault(move => move.MoveUid == @event.MoveId.ToGuid());
    if (move is null)
    {
      return false;
    }

    move.Remember(@event);
    return true;
  }

  public void RemoveItem(PokemonItemRemoved @event)
  {
    Update(@event);

    HeldItem = null;
    HeldItemId = null;
    HeldItemUid = null;
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

  public void SwitchMoves(PokemonMoveSwitched @event)
  {
    Update(@event);

    PokemonMoveEntity? source = Moves.SingleOrDefault(x => x.Position == @event.Source);
    PokemonMoveEntity? destination = Moves.SingleOrDefault(x => x.Position == @event.Destination);
    if (source is not null && destination is not null)
    {
      source.Switch(destination);
    }
  }

  public void Update(PokemonUpdated @event)
  {
    base.Update(@event);

    if (@event.IsShiny.HasValue)
    {
      IsShiny = @event.IsShiny.Value;
    }

    if (@event.Vitality.HasValue)
    {
      Vitality = @event.Vitality.Value;
    }
    if (@event.Stamina.HasValue)
    {
      Stamina = @event.Stamina.Value;
    }
    if (@event.StatusCondition is not null)
    {
      StatusCondition = @event.StatusCondition.Value;
    }
    if (@event.Friendship is not null)
    {
      Friendship = @event.Friendship.Value;
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

  private void RemoveMove(int? position)
  {
    if (position.HasValue)
    {
      PokemonMoveEntity? pokemonMove = Moves.SingleOrDefault(move => move.Position == position.Value);
      pokemonMove?.Remove();
    }
  }

  private void SetOwnership(TrainerEntity trainer, ItemEntity pokeBall, OwnershipKind kind, IPokemonOwnershipEvent @event, DateTime occurredOn)
  {
    if (kind == Core.Pokemon.OwnershipKind.Caught || !OriginalTrainerId.HasValue)
    {
      SetOriginalTrainer(trainer);
    }
    SetCurrentTrainer(trainer);
    SetPokeBall(pokeBall);

    OwnershipKind = kind;
    MetAtLevel = @event.Level.Value;
    MetLocation = @event.Location.Value;
    MetOn = (@event.MetOn ?? occurredOn).AsUniversalTime();
    MetDescription = @event.Description?.Value;

    SetSlot(@event.Slot);
  }
  private void SetCurrentTrainer(TrainerEntity? trainer)
  {
    CurrentTrainer = trainer;
    CurrentTrainerId = trainer?.TrainerId;
    CurrentTrainerUid = trainer?.Id;
  }
  private void SetOriginalTrainer(TrainerEntity? trainer)
  {
    OriginalTrainer = trainer;
    OriginalTrainerId = trainer?.TrainerId;
    OriginalTrainerUid = trainer?.Id;
  }
  private void SetPokeBall(ItemEntity? pokeBall)
  {
    PokeBall = pokeBall;
    PokeBallId = pokeBall?.ItemId;
    PokeBallUid = pokeBall?.Id;
  }
  private void SetSlot(PokemonSlot? slot)
  {
    Position = slot?.Position.Value;
    Box = slot?.Box?.Value;
  }

  public PokemonStatisticsModel GetStatistics()
  {
    string[] properties = Statistics.Split('|');
    byte[] baseStatistics = properties[0].Split(',').Select(byte.Parse).ToArray();
    byte[] individualValues = properties[1].Split(',').Select(byte.Parse).ToArray();
    byte[] effortValues = properties[2].Split(',').Select(byte.Parse).ToArray();
    int[] values = properties[3].Split(',').Select(int.Parse).ToArray();
    return new PokemonStatisticsModel(
      new PokemonStatisticModel(baseStatistics[0], individualValues[0], effortValues[0], values[0]),
      new PokemonStatisticModel(baseStatistics[1], individualValues[1], effortValues[1], values[1]),
      new PokemonStatisticModel(baseStatistics[2], individualValues[2], effortValues[2], values[2]),
      new PokemonStatisticModel(baseStatistics[3], individualValues[3], effortValues[3], values[3]),
      new PokemonStatisticModel(baseStatistics[4], individualValues[4], effortValues[4], values[4]),
      new PokemonStatisticModel(baseStatistics[5], individualValues[5], effortValues[5], values[5]));
  }
  private void SetStatistics(IBaseStatistics? baseStatistics = null, IIndividualValues? individualValues = null, IEffortValues? effortValues = null)
  {
    if (baseStatistics is null || individualValues is null || effortValues is null)
    {
      PokemonStatisticsModel statistics = GetStatistics();
      baseStatistics ??= new BaseStatistics(statistics.HP.Base, statistics.Attack.Base, statistics.Defense.Base,
        statistics.SpecialAttack.Base, statistics.SpecialDefense.Base, statistics.Speed.Base);
      individualValues ??= new IndividualValues(statistics.HP.IndividualValue, statistics.Attack.IndividualValue, statistics.Defense.IndividualValue,
        statistics.SpecialAttack.IndividualValue, statistics.SpecialDefense.IndividualValue, statistics.Speed.IndividualValue);
      effortValues ??= new EffortValues(statistics.HP.EffortValue, statistics.Attack.EffortValue, statistics.Defense.EffortValue,
        statistics.SpecialAttack.EffortValue, statistics.SpecialDefense.EffortValue, statistics.Speed.EffortValue);
    }

    PokemonStatistics values = new(baseStatistics, individualValues, effortValues, Level, PokemonNatures.Instance.Find(Nature));
    Statistics = string.Join('|',
      string.Join(',', baseStatistics.HP, baseStatistics.Attack, baseStatistics.Defense, baseStatistics.SpecialAttack, baseStatistics.SpecialDefense, baseStatistics.Speed),
      string.Join(',', individualValues.HP, individualValues.Attack, individualValues.Defense, individualValues.SpecialAttack, individualValues.SpecialDefense, individualValues.Speed),
      string.Join(',', effortValues.HP, effortValues.Attack, effortValues.Defense, effortValues.SpecialAttack, effortValues.SpecialDefense, effortValues.Speed),
      string.Join(',', values.HP, values.Attack, values.Defense, values.SpecialAttack, values.SpecialDefense, values.Speed));
  }

  public override string ToString() => $"{Nickname ?? UniqueName}";
}
