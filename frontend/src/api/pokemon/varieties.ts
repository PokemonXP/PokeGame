import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { SearchVarietiesPayload } from "@/types/pokemon/varieties";
import type { Variety } from "@/types/pokemon";
import { get } from "..";

export async function searchVarieties(speciesId: string, payload: SearchVarietiesPayload): Promise<SearchResults<Variety>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/{speciesId}/varieties" })
    .setParameter("speciesId", speciesId)
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
