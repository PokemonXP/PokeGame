type TypeEffectiveness = {
  [attackingType in PokemonType]: {
    [defendingType in PokemonType]: number;
  };
};
import effectiveness from "@/resources/type-effectiveness.json" assert { type: "json" };
const effectivenessChart: TypeEffectiveness = effectiveness;

import { EFFORT_VALUE_MAXIMUM, EFFORT_VALUE_MINIMUM, INDIVIDUAL_VALUE_MAXIMUM, INDIVIDUAL_VALUE_MINIMUM, LEVEL_MAXIMUM, LEVEL_MINIMUM } from "@/types/pokemon";
import type { Ability } from "@/types/abilities";
import type { BaseStatistics, Form, FormAbilities, Sprites } from "@/types/pokemon-forms";
import type {
  EffortValues,
  IndividualValues,
  Pokemon,
  PokemonNature,
  PokemonSize,
  PokemonSizeCategory,
  PokemonStatistic,
  PokemonStatistics,
  PokemonType,
  StatisticValues,
} from "@/types/pokemon";
import type { GrowthRate } from "@/types/species";

function calculateErratic(level: number): number {
  let experience: number = 0;
  const cube: number = Math.pow(level, 3);
  if (level < 50) {
    experience = (cube * (100 - level)) / 50;
  } else if (level < 68) {
    experience = (cube * (150 - level)) / 100;
  } else if (level < 98) {
    experience = (cube * Math.floor((1911 - 10 * level) / 3)) / 500;
  } else {
    experience = (cube * (160 - level)) / 100;
  }
  return Math.floor(experience);
}
function calculateFast(level: number): number {
  return Math.floor((4 * Math.pow(level, 3)) / 5);
}
function calculateFluctuating(level: number): number {
  let experience: number = 0;
  const cube: number = Math.pow(level, 3);
  if (level < 15) {
    experience = (cube * (Math.floor((level + 1) / 3) + 24)) / 50;
  } else if (level < 36) {
    experience = (cube * (level + 14)) / 50;
  } else {
    experience = (cube * (Math.floor(level / 2) + 32)) / 50;
  }
  return Math.floor(experience);
}
function calculateMediumFast(level: number): number {
  return Math.floor(Math.pow(level, 3));
}
function calculateMediumSlow(level: number): number {
  return Math.floor((6 * Math.pow(level, 3)) / 5 - 15 * Math.pow(level, 2) + 100 * level - 140);
}
function calculateSlow(level: number): number {
  return Math.floor((5 * Math.pow(level, 3)) / 4);
}

const erratic: number[] = [0];
const fast: number[] = [0];
const fluctuating: number[] = [0];
const mediumFast: number[] = [0];
const mediumSlow: number[] = [0];
const slow: number[] = [0];
for (let level = 2; level <= LEVEL_MAXIMUM; level++) {
  erratic.push(calculateErratic(level));
  fast.push(calculateFast(level));
  fluctuating.push(calculateFluctuating(level));
  mediumFast.push(calculateMediumFast(level));
  mediumSlow.push(calculateMediumSlow(level));
  slow.push(calculateSlow(level));
}
const _thresholds = new Map<GrowthRate, number[]>([
  ["Erratic", erratic],
  ["Fast", fast],
  ["Fluctuating", fluctuating],
  ["MediumFast", mediumFast],
  ["MediumSlow", mediumSlow],
  ["Slow", slow],
]);

export function calculateSize(form: Form, scalars: PokemonSize): PokemonSize {
  const heightMultiplier: number = (scalars.height / 255) * 0.4 + 0.8;
  return {
    height: Math.floor(heightMultiplier * form.height) / 10,
    weight: Math.floor(((scalars.weight / 255) * 0.4 + 0.8) * heightMultiplier * form.weight) / 10,
    category: getSizeCategory(scalars.height),
  };
}

function calculateHP(base: number, individual: number, effort: number, level: number): StatisticValues {
  if (individual < INDIVIDUAL_VALUE_MINIMUM || individual > INDIVIDUAL_VALUE_MAXIMUM) {
    individual = 0;
  }
  if (effort < EFFORT_VALUE_MINIMUM || effort > EFFORT_VALUE_MAXIMUM) {
    effort = 0;
  }
  if (level < LEVEL_MINIMUM) {
    level = 1;
  }
  if (level > LEVEL_MAXIMUM) {
    level = 100;
  }
  const value: number = Math.floor(((2 * base + individual + Math.floor(effort / 4)) * level) / 100) + level + 10;
  return {
    base,
    individualValue: individual,
    effortValue: effort,
    value,
  };
}
function calculateStatistic(base: number, individual: number, effort: number, level: number, nature: number): StatisticValues {
  if (individual < INDIVIDUAL_VALUE_MINIMUM || individual > INDIVIDUAL_VALUE_MAXIMUM) {
    individual = 0;
  }
  if (effort < EFFORT_VALUE_MINIMUM || effort > EFFORT_VALUE_MAXIMUM) {
    effort = 0;
  }
  if (level < LEVEL_MINIMUM) {
    level = 1;
  }
  if (level > LEVEL_MAXIMUM) {
    level = 100;
  }
  const value: number = Math.floor((Math.floor(((2 * base + individual + Math.floor(effort / 4)) * level) / 100) + 5) * nature);
  return {
    base,
    individualValue: individual,
    effortValue: effort,
    value,
  };
}
function getNatureMultiplier(statistic: PokemonStatistic, nature: PokemonNature): number {
  if (statistic === nature.increasedStatistic && statistic !== nature.decreasedStatistic) {
    return 1.1;
  } else if (statistic === nature.decreasedStatistic) {
    return 0.9;
  }
  return 1;
}
export function calculateStatistics(
  base: BaseStatistics,
  individual: IndividualValues,
  effort: EffortValues,
  level: number,
  nature: PokemonNature,
): PokemonStatistics {
  return {
    hp: calculateHP(base.hp, individual.hp, effort.hp, level),
    attack: calculateStatistic(base.attack, individual.attack, effort.attack, level, getNatureMultiplier("Attack", nature)),
    defense: calculateStatistic(base.defense, individual.defense, effort.defense, level, getNatureMultiplier("Defense", nature)),
    specialAttack: calculateStatistic(base.specialAttack, individual.specialAttack, effort.specialAttack, level, getNatureMultiplier("SpecialAttack", nature)),
    specialDefense: calculateStatistic(
      base.specialDefense,
      individual.specialDefense,
      effort.specialDefense,
      level,
      getNatureMultiplier("SpecialDefense", nature),
    ),
    speed: calculateStatistic(base.speed, individual.speed, effort.speed, level, getNatureMultiplier("Speed", nature)),
  };
}

export function getLevel(growthRate: GrowthRate, experience: number): number {
  const thresholds: number[] | undefined = _thresholds.get(growthRate);
  if (!thresholds) {
    throw new Error(`The threshold table was not found for growth rate '${growthRate}'.`);
  }
  if (experience < 0) {
    throw new Error("The experience must be greater than or equal to 0.");
  }

  for (let level = 0; level < LEVEL_MAXIMUM; level++) {
    if (experience < thresholds[level]) {
      return level;
    }
  }

  return LEVEL_MAXIMUM;
}

export function getMaximumExperience(growthRate: GrowthRate, level: number): number {
  const thresholds: number[] | undefined = _thresholds.get(growthRate);
  if (!thresholds) {
    throw new Error(`The threshold table was not found for growth rate '${growthRate}'.`);
  }
  if (level < LEVEL_MINIMUM || level > LEVEL_MAXIMUM) {
    throw new Error(`The level must be comprised between ${LEVEL_MINIMUM} and ${LEVEL_MAXIMUM}.`);
  }

  return thresholds[Math.min(level, LEVEL_MAXIMUM - 1)];
}

export function getSizeCategory(heightScalar: number): PokemonSizeCategory {
  if (heightScalar <= 15) {
    return "ExtraSmall";
  } else if (heightScalar <= 47) {
    return "Small";
  } else if (heightScalar >= 240) {
    return "ExtraLarge";
  } else if (heightScalar >= 208) {
    return "Large";
  }
  return "Medium";
}

export function getSpriteUrl(pokemon: Pokemon): string {
  if (pokemon.isEgg) {
    return "/img/egg.png";
  } else if (pokemon.sprite) {
    return pokemon.sprite;
  }
  const sprites: Sprites = pokemon.form.sprites;
  if (pokemon.isShiny) {
    return sprites.alternativeShiny && pokemon.gender === "Female" ? sprites.alternativeShiny : sprites.shiny;
  }
  return sprites.alternative && pokemon.gender === "Female" ? sprites.alternative : sprites.default;
}

export function getAbility(pokemon: Pokemon): Ability {
  const abilities: FormAbilities = pokemon.form.abilities;
  switch (pokemon.abilitySlot) {
    case "Secondary":
      if (abilities.secondary) {
        return abilities.secondary;
      }
      break;
    case "Hidden":
      if (abilities.hidden) {
        return abilities.hidden;
      }
      break;
  }
  return abilities.primary;
}

export function getUrl(pokemon: Pokemon): string | undefined {
  return pokemon.url ?? pokemon.form.url ?? pokemon.form.variety.url ?? pokemon.form.variety.species.url ?? undefined;
}

export function calculateStamina(powerPoints: number): number {
  return Math.round(714 / 4 / powerPoints); // NOTE(fpion): highest HP possible value divided by maximum number of Pokémon moves.
}

export function getTypeEffectiveness(attacker: PokemonType, defender: PokemonType): number {
  return effectivenessChart[attacker][defender];
}

/**
 * Calculates the accuracy of a move
 * @param move The accuracy of the move, a number between 1 and 100 (inclusive).
 * @param accuracy The attacker accuracy stages, a number between -6 and +6 (inclusive).
 * @param evasion The target evasion stages, a number between -6 and +6 (inclusive).
 */
export function calculateAccuracy(move: number, accuracy: number, evasion: number): number {
  if (move < 1 || move > 100 || accuracy === evasion) {
    return move;
  }

  let result: number = 0;
  const stages: number = accuracy - evasion;
  if (stages > 0) {
    result = (move * Math.min(3 + stages, 9)) / 3;
  } else {
    result = (move * 3) / Math.min(3 - stages, 9);
  }
  result = Math.floor(result);

  return result < 1 ? 1 : result > 100 ? 100 : result;
}

export function calculateDamage(
  level: number,
  power: number,
  attack: number,
  defense: number,
  targets: number,
  critical: number,
  random: number,
  stab: number,
  effectiveness: number,
  other: number,
): number {
  // NOTE(fpion): base damage calculation
  let damage: number = (2 * level) / 5 + 2;
  damage *= power;
  damage *= attack;
  damage /= defense;
  damage /= 50;
  damage += 2;
  // NOTE(fpion): multipliers
  damage *= targets;
  damage *= critical;
  damage *= random;
  damage *= stab;
  damage *= effectiveness;
  damage *= other;
  // NOTE(fpion): final step
  return Math.max(Math.floor(damage), 0);
}
