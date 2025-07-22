import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { CreateOrReplaceAbilityPayload, Ability, SearchAbilitiesPayload, UpdateAbilityPayload } from "@/types/abilities";
import { _delete, get, patch, post } from ".";

export async function createAbility(payload: CreateOrReplaceAbilityPayload): Promise<Ability> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities" }).buildRelative();
  return (await post<CreateOrReplaceAbilityPayload, Ability>(url, payload)).data;
}

export async function deleteAbility(id: string): Promise<Ability> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Ability>(url)).data;
}

export async function readAbility(id: string): Promise<Ability> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Ability>(url)).data;
}

export async function searchAbilities(payload: SearchAbilitiesPayload): Promise<SearchResults<Ability>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities" })
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
  return (await get<SearchResults<Ability>>(url)).data;
}

export async function updateAbility(id: string, payload: UpdateAbilityPayload): Promise<Ability> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateAbilityPayload, Ability>(url, payload)).data;
}
