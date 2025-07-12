import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Move, SearchMovesPayload } from "@/types/pokemon/moves";
import { get } from "..";

export async function searchMoves(payload: SearchMovesPayload): Promise<SearchResults<Move>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves" })
    .setQuery("type", payload.type ?? "")
    .setQuery("category", payload.category ?? "")
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
  return (await get<SearchResults<Move>>(url)).data;
}
