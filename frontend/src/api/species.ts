import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { CreateOrReplaceSpeciesPayload, Species, SearchSpeciesPayload, UpdateSpeciesPayload } from "@/types/species";
import { _delete, get, patch, post } from ".";

export async function createSpecies(payload: CreateOrReplaceSpeciesPayload): Promise<Species> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species" }).buildRelative();
  return (await post<CreateOrReplaceSpeciesPayload, Species>(url, payload)).data;
}

export async function deleteSpecies(id: string): Promise<Species> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Species>(url)).data;
}

export async function readSpecies(id: string): Promise<Species> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Species>(url)).data;
}

export async function searchSpecies(payload: SearchSpeciesPayload): Promise<SearchResults<Species>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species" })
    .setQuery("category", payload.category ?? "")
    .setQuery("egg", payload.eggGroup ?? "")
    .setQuery("growth", payload.growthRate ?? "")
    .setQuery("region", payload.regionId ?? "")
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Species>>(url)).data;
}

export async function updateSpecies(id: string, payload: UpdateSpeciesPayload): Promise<Species> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateSpeciesPayload, Species>(url, payload)).data;
}
