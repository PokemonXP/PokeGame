import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Item, SearchItemsPayload } from "@/types/items";
import { get } from ".";

export async function searchItems(payload: SearchItemsPayload): Promise<SearchResults<Item>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items" })
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
  return (await get<SearchResults<Item>>(url)).data;
}
