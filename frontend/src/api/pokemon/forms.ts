import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Form } from "@/types/pokemon";
import { get } from "..";
import type { SearchFormsPayload } from "@/types/pokemon/forms";

export async function searchForms(varietyId: string, payload: SearchFormsPayload): Promise<SearchResults<Form>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{varietyId}/forms" })
    .setParameter("varietyId", varietyId)
    .setQuery("type", payload.type ?? "")
    .setQuery("ability", payload.abilityId ?? "")
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
  return (await get<SearchResults<Form>>(url)).data;
}
