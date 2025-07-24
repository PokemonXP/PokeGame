import type { PokemonGender, PokemonType } from ".";

export type ItemSummary = {
  name: string;
  sprite?: string | null;
};

export type PokemonSheet = {
  id: string;
  name: string;
  gender?: PokemonGender | null;
  sprite: string;
  level: number;
  constitution: number;
  vitality: number;
  stamina: number;
  heldItem?: ItemSummary;
};

export type PokemonSummary = {
  id: string;
  number: number;
  name: string;
  nickname?: string | null;
  gender?: PokemonGender | null;
  sprite: string;
  primaryType: PokemonType;
  secondaryType: PokemonType | null;
  teraType: PokemonType;
  level: number;
  experience: number;
  heldItem?: ItemSummary | null;
  originalTrainer?: TrainerSummary | null;
  pokeBall: ItemSummary;
};

export type TrainerSummary = {
  name: string;
  sprite?: string | null;
};

// TODO(fpion): move one directory top
// TODO(fpion): move game trainer types in this file
