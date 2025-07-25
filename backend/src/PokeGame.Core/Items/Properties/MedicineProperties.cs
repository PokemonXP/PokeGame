﻿using FluentValidation;
using PokeGame.Core.Items.Validators;

namespace PokeGame.Core.Items.Properties;

public record MedicineProperties : ItemProperties, IMedicineProperties
{
  [JsonInclude]
  public override ItemCategory Category { get; } = ItemCategory.Medicine;

  public bool IsHerbal { get; }

  public int Healing { get; }
  public bool IsHealingPercentage { get; }
  public bool Revives { get; }

  public StatusCondition? StatusCondition { get; }
  public bool AllConditions { get; }

  public int PowerPoints { get; }
  public bool IsPowerPointPercentage { get; }
  public bool RestoreAllMoves { get; }

  public MedicineProperties()
  {
  }

  [JsonConstructor]
  public MedicineProperties(
    bool isHerbal,
    int healing,
    bool isHealingPercentage,
    bool revives,
    StatusCondition? statusCondition,
    bool allConditions,
    int powerPoints,
    bool isPowerPointPercentage,
    bool restoreAllMoves)
  {
    IsHerbal = isHerbal;

    Healing = healing;
    IsHealingPercentage = isHealingPercentage;
    Revives = revives;

    StatusCondition = statusCondition;
    AllConditions = allConditions;

    PowerPoints = powerPoints;
    IsPowerPointPercentage = isPowerPointPercentage;
    RestoreAllMoves = restoreAllMoves;

    new MedicineValidator().ValidateAndThrow(this);
  }

  public MedicineProperties(IMedicineProperties medicine) : this(
    medicine.IsHerbal,
    medicine.Healing,
    medicine.IsHealingPercentage,
    medicine.Revives,
    medicine.StatusCondition,
    medicine.AllConditions,
    medicine.PowerPoints,
    medicine.IsPowerPointPercentage,
    medicine.RestoreAllMoves)
  {
  }
}
