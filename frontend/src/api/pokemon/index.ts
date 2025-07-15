import { urlUtils } from "logitar-js";

import type {
  CreatePokemonPayload,
  Pokemon,
  RelearnPokemonMovePayload,
  SearchPokemonPayload,
  SwitchPokemonMovesPayload,
  UpdatePokemonPayload,
} from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from "..";

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

export async function relearnPokemonMove(id: string, payload: RelearnPokemonMovePayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/moves/relearn" }).setParameter("id", id).buildRelative();
  return (await put<RelearnPokemonMovePayload, Pokemon>(url, payload)).data;
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

export async function switchPokemonMoves(id: string, payload: SwitchPokemonMovesPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/moves/switch" }).setParameter("id", id).buildRelative();
  return (await put<SwitchPokemonMovesPayload, Pokemon>(url, payload)).data;
}

export async function updatePokemon(id: string, payload: UpdatePokemonPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdatePokemonPayload, Pokemon>(url, payload)).data;
}
