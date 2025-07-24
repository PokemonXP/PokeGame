import type { PokemonGender, PokemonSizeCategory, PokemonType } from ".";

export type ExperienceSummary = {
  current: number
  minimum: number
  maximum: number
  toNextLevel: number
  percentage: number
}

export type ItemCard = {
  name: string;
  sprite?: string | null;
};

export type ItemSummary = {
  name: string;
  description?: string | null;
  sprite?: string | null;
};

export type PokemonBase = {
  name: string;
  sprite: string;
  level: number;
};

export type PokemonCard = PokemonBase & {
  id: string;
  gender?: PokemonGender | null;
  constitution: number;
  vitality: number;
  stamina: number;
  heldItem?: ItemCard;
};

export type PokemonSummary = PokemonBase & {
  id: string;
  number: number;
  nickname?: string | null;
  gender?: PokemonGender | null;
  primaryType: PokemonType;
  secondaryType: PokemonType | null;
  teraType: PokemonType;
  height: number;
  weight: number;
  size: PokemonSizeCategory;
  experience?: ExperienceSummary | null;
  heldItem?: ItemSummary | null;
  originalTrainer?: TrainerSummary | null;
  caughtBallSprite?: string | null;
};

export type TrainerSummary = {
  license: string;
  name: string;
};

// TODO(fpion): move one directory top
// TODO(fpion): move game trainer types in this file

