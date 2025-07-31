import { urlUtils } from "logitar-js";

import type { CreateBattlePayload, Battle, SearchBattlesPayload, UpdateBattlePayload, SwitchBattlePokemonPayload } from "@/types/battle";
import { _delete, get, patch, post } from ".";
import type { SearchResults } from "@/types/search";

export async function cancelBattle(id: string): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles/{id}/cancel" }).setParameter("id", id).buildRelative();
  return (await patch<void, Battle>(url)).data;
}

export async function createBattle(payload: CreateBattlePayload): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles" }).buildRelative();
  return (await post<CreateBattlePayload, Battle>(url, payload)).data;
}

export async function deleteBattle(id: string): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Battle>(url)).data;
}

export async function escapeBattle(id: string): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles/{id}/escape" }).setParameter("id", id).buildRelative();
  return (await patch<void, Battle>(url)).data;
}

export async function readBattle(id: string): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Battle>(url)).data;
}

export async function resetBattle(id: string): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles/{id}/reset" }).setParameter("id", id).buildRelative();
  return (await patch<void, Battle>(url)).data;
}

export async function searchBattles(payload: SearchBattlesPayload): Promise<SearchResults<Battle>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles" })
    .setQuery("kind", payload.kind ?? "")
    .setQuery("status", payload.status ?? "")
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
  return (await get<SearchResults<Battle>>(url)).data;
}

export async function startBattle(id: string): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles/{id}/start" }).setParameter("id", id).buildRelative();
  return (await patch<void, Battle>(url)).data;
}

export async function switchBattlePokemon(id: string, payload: SwitchBattlePokemonPayload): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles/{id}/pokemon/switch" }).setParameter("id", id).buildRelative();
  return (await patch<SwitchBattlePokemonPayload, Battle>(url, payload)).data;
}

export async function updateBattle(id: string, payload: UpdateBattlePayload): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateBattlePayload, Battle>(url, payload)).data;
}
