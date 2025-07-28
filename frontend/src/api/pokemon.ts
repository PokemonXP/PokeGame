import { urlUtils } from "logitar-js";

import type {
  CatchPokemonPayload,
  ChangePokemonFormPayload,
  CreatePokemonPayload,
  MovePokemonPayload,
  Pokemon,
  ReceivePokemonPayload,
  RememberPokemonMovePayload,
  SearchPokemonPayload,
  SwapPokemonPayload,
  SwitchPokemonMovesPayload,
  UpdatePokemonPayload,
} from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post } from ".";

export async function catchPokemon(id: string, payload: CatchPokemonPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/catch" }).setParameter("id", id).buildRelative();
  return (await patch<CatchPokemonPayload, Pokemon>(url, payload)).data;
}

export async function changePokemonForm(id: string, payload: ChangePokemonFormPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/form" }).setParameter("id", id).buildRelative();
  return (await patch<ChangePokemonFormPayload, Pokemon>(url, payload)).data;
}

export async function depositPokemon(id: string): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/deposit" }).setParameter("id", id).buildRelative();
  return (await patch<void, Pokemon>(url)).data;
}

export async function createPokemon(payload: CreatePokemonPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon" }).buildRelative();
  return (await post<CreatePokemonPayload, Pokemon>(url, payload)).data;
}

export async function deletePokemon(id: string): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Pokemon>(url)).data;
}

export async function healPokemon(id: string): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/heal" }).setParameter("id", id).buildRelative();
  return (await patch<void, Pokemon>(url)).data;
}

export async function movePokemon(id: string, payload: MovePokemonPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/move" }).setParameter("id", id).buildRelative();
  return (await patch<MovePokemonPayload, Pokemon>(url, payload)).data;
}

export async function readPokemon(id: string): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Pokemon>(url)).data;
}

export async function receivePokemon(id: string, payload: ReceivePokemonPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/receive" }).setParameter("id", id).buildRelative();
  return (await patch<ReceivePokemonPayload, Pokemon>(url, payload)).data;
}

export async function releasePokemon(id: string): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/release" }).setParameter("id", id).buildRelative();
  return (await patch<void, Pokemon>(url)).data;
}

export async function rememberPokemonMove(pokemonId: string, moveId: string, payload: RememberPokemonMovePayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{pokemonId}/moves/{moveId}/remember" })
    .setParameter("pokemonId", pokemonId)
    .setParameter("moveId", moveId)
    .buildRelative();
  return (await patch<RememberPokemonMovePayload, Pokemon>(url, payload)).data;
}

export async function searchPokemon(payload: SearchPokemonPayload): Promise<SearchResults<Pokemon>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon" })
    .setQuery("species", payload.speciesId ?? "")
    .setQuery("item", payload.heldItemId ?? "")
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

export async function swapPokemon(payload: SwapPokemonPayload): Promise<Pokemon[]> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/swap" }).buildRelative();
  return (await patch<SwapPokemonPayload, Pokemon[]>(url, payload)).data;
}

export async function switchPokemonMoves(id: string, payload: SwitchPokemonMovesPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/moves/switch" }).setParameter("id", id).buildRelative();
  return (await patch<SwitchPokemonMovesPayload, Pokemon>(url, payload)).data;
}

export async function updatePokemon(id: string, payload: UpdatePokemonPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdatePokemonPayload, Pokemon>(url, payload)).data;
}

export async function withdrawPokemon(id: string): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}/withdraw" }).setParameter("id", id).buildRelative();
  return (await patch<void, Pokemon>(url)).data;
}
