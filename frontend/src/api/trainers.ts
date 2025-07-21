import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { SearchTrainersPayload, Trainer } from "@/types/trainers";
import { get } from ".";

export async function searchTrainers(payload: SearchTrainersPayload): Promise<SearchResults<Trainer>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers" })
    .setQuery("gender", payload.gender ?? "")
    .setQuery("user", payload.userId ?? "")
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
  return (await get<SearchResults<Trainer>>(url)).data;
}
