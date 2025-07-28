import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { CreateOrReplaceVarietyPayload, SearchVarietiesPayload, UpdateVarietyPayload, Variety } from "@/types/varieties";
import { _delete, get, patch, post } from ".";

export async function createVariety(payload: CreateOrReplaceVarietyPayload): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties" }).buildRelative();
  return (await post<CreateOrReplaceVarietyPayload, Variety>(url, payload)).data;
}

export async function deleteVariety(id: string): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Variety>(url)).data;
}

export async function readVariety(id: string): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Variety>(url)).data;
}

export async function searchVarieties(payload: SearchVarietiesPayload): Promise<SearchResults<Variety>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties" })
    .setQuery("species", payload.speciesId ?? "")
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
  return (await get<SearchResults<Variety>>(url)).data;
}

export async function updateVariety(id: string, payload: UpdateVarietyPayload): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateVarietyPayload, Variety>(url, payload)).data;
}
