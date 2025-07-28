import { urlUtils } from "logitar-js";

import type { CreateOrReplaceFormPayload, Form } from "@/types/pokemon-forms";
import type { SearchFormsPayload, UpdateFormPayload } from "@/types/pokemon-forms";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post } from ".";

export async function createForm(payload: CreateOrReplaceFormPayload): Promise<Form> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms" }).buildRelative();
  return (await post<CreateOrReplaceFormPayload, Form>(url, payload)).data;
}

export async function deleteForm(id: string): Promise<Form> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Form>(url)).data;
}

export async function readForm(id: string): Promise<Form> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Form>(url)).data;
}

export async function searchForms(payload: SearchFormsPayload): Promise<SearchResults<Form>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms" })
    .setQuery("variety", payload.varietyId ?? "")
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

export async function updateForm(id: string, payload: UpdateFormPayload): Promise<Form> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateFormPayload, Form>(url, payload)).data;
}
