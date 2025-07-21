import { stringUtils } from "logitar-js";

import type { Ability, Form, Species, Trainer, Variety } from "@/types/pokemon";
import type { Item } from "@/types/items";
import type { Move } from "@/types/pokemon/moves";

const cmsBaseUrl: string = import.meta.env.VITE_APP_CMS_BASE_URL ?? "";
const { trimEnd } = stringUtils;

export const ContentTypes: Record<string, string> = {
  Abilities: "748ded48-8f53-4e17-8524-e098a468daf4",
  BattleItems: "e2712045-480f-40ed-81ee-f44d26def685",
  Berries: "0be02b4f-7e27-479e-b119-7790d582b80f",
  Forms: "9eb068a7-6b10-4fbc-b722-11ba522c00f5",
  Items: "f03d8207-3469-489c-8cef-ed2f55672048",
  Medicines: "07ca76cf-d73c-44d8-926e-9ead192427c0",
  Moves: "ee1f0b92-6970-4d27-aa8f-b2e779608ecc",
  PokeBalls: "4374a286-2ea2-433c-bbba-76b0e882210a",
  Regions: "4bd6ba59-6d0e-4a61-8d6e-cc65deb6baab",
  Species: "da10e8b3-6284-487a-9e48-48b8f356e4b7",
  TechnicalMachines: "7ea2356b-a0b5-45e3-a814-5fc71d5241dd",
  Trainers: "660f4f93-66b2-4a6a-87e0-1b07e8f5fdd7",
  Varieties: "31ae9c83-d4a1-44cf-9007-c614c68cc1d3",
};

export function getContentListUrl(contentType: string): string {
  return `${cmsBaseUrl}/admin/contents?type=${contentType}`;
}

export function getAbilityUrl(ability: Ability): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${ability.id}`;
}
export function getFormUrl(form: Form): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${form.id}`;
}
export function getItemUrl(item: Item): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${item.id}`;
}
export function getMoveUrl(move: Move): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${move.id}`;
}
export function getSpeciesUrl(species: Species): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${species.id}`;
}
export function getTrainerUrl(trainer: Trainer): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${trainer.id}`;
}
export function getVarietyUrl(variety: Variety): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${variety.id}`;
}

// TODO(fpion): remove this file
