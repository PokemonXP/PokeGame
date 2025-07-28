import { urlUtils } from "logitar-js";

import type { CreateOrReplaceEvolutionPayload, Evolution, SearchEvolutionsPayload, UpdateEvolutionPayload } from "@/types/evolutions";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post } from ".";

export async function createEvolution(payload: CreateOrReplaceEvolutionPayload): Promise<Evolution> {
  const url: string = new urlUtils.UrlBuilder({ path: "/evolutions" }).buildRelative();
  return (await post<CreateOrReplaceEvolutionPayload, Evolution>(url, payload)).data;
}

export async function deleteEvolution(id: string): Promise<Evolution> {
  const url: string = new urlUtils.UrlBuilder({ path: "/evolutions/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Evolution>(url)).data;
}

export async function readEvolution(id: string): Promise<Evolution> {
  const url: string = new urlUtils.UrlBuilder({ path: "/evolutions/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Evolution>(url)).data;
}

export async function searchEvolutions(payload: SearchEvolutionsPayload): Promise<SearchResults<Evolution>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/evolutions" })
    .setQuery("source", payload.sourceId ?? "")
    .setQuery("target", payload.targetId ?? "")
    .setQuery("trigger", payload.trigger ?? "")
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
  return (await get<SearchResults<Evolution>>(url)).data;
}

export async function updateEvolution(id: string, payload: UpdateEvolutionPayload): Promise<Evolution> {
  const url: string = new urlUtils.UrlBuilder({ path: "/evolutions/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateEvolutionPayload, Evolution>(url, payload)).data;
}
