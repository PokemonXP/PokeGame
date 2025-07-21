import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { CreateOrReplaceRegionPayload, Region, SearchRegionsPayload, UpdateRegionPayload } from "@/types/regions";
import { _delete, get, patch, post } from ".";

export async function createRegion(payload: CreateOrReplaceRegionPayload): Promise<Region> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions" }).buildRelative();
  return (await post<CreateOrReplaceRegionPayload, Region>(url, payload)).data;
}

export async function deleteRegion(id: string): Promise<Region> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Region>(url)).data;
}

export async function readRegion(id: string): Promise<Region> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Region>(url)).data;
}

export async function searchRegions(payload: SearchRegionsPayload): Promise<SearchResults<Region>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions" })
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
  return (await get<SearchResults<Region>>(url)).data;
}

export async function updateRegion(id: string, payload: UpdateRegionPayload): Promise<Region> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateRegionPayload, Region>(url, payload)).data;
}
