import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { SearchSpeciesPayload } from "@/types/pokemon/species";
import type { Species } from "@/types/pokemon";
import { get } from "..";

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
