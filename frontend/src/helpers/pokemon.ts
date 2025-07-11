import { LEVEL_MAXIMUM, LEVEL_MINIMUM } from "@/types/pokemon";
import type { GrowthRate } from "@/types/pokemon";

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
