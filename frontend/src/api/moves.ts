import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { CreateOrReplaceMovePayload, Move, SearchMovesPayload, UpdateMovePayload } from "@/types/moves";
import { _delete, get, patch, post } from ".";

export async function createMove(payload: CreateOrReplaceMovePayload): Promise<Move> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves" }).buildRelative();
  return (await post<CreateOrReplaceMovePayload, Move>(url, payload)).data;
}

export async function deleteMove(id: string): Promise<Move> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Move>(url)).data;
}

export async function readMove(id: string): Promise<Move> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Move>(url)).data;
}

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

export async function updateMove(id: string, payload: UpdateMovePayload): Promise<Move> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateMovePayload, Move>(url, payload)).data;
}
