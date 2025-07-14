import { urlUtils } from "logitar-js";

import type { CreatePokemonPayload, Pokemon, SearchPokemonPayload, UpdatePokemonPayload } from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post } from "..";

export async function createPokemon(payload: CreatePokemonPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon" }).buildRelative();
  return (await post<CreatePokemonPayload, Pokemon>(url, payload)).data;
}

export async function deletePokemon(id: string): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Pokemon>(url)).data;
}

export async function readPokemon(id: string): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Pokemon>(url)).data;
}

export async function searchPokemon(payload: SearchPokemonPayload): Promise<SearchResults<Pokemon>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon" })
    .setQuery("trainer", payload.trainerId ?? "")
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
  return (await get<SearchResults<Pokemon>>(url)).data;
}

export async function updatePokemon(id: string, payload: UpdatePokemonPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdatePokemonPayload, Pokemon>(url, payload)).data;
}
