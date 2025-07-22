import { stringUtils } from "logitar-js";

import type { Form, Species, Variety } from "@/types/pokemon";
import type { Move } from "@/types/pokemon/moves";

const cmsBaseUrl: string = import.meta.env.VITE_APP_CMS_BASE_URL ?? "";
const { trimEnd } = stringUtils;

export function getFormUrl(form: Form): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${form.id}`;
}
export function getMoveUrl(move: Move): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${move.id}`;
}
export function getSpeciesUrl(species: Species): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${species.id}`;
}
export function getVarietyUrl(variety: Variety): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${variety.id}`;
}

// TODO(fpion): remove this file
