import { urlUtils } from "logitar-js";

import type { CreateOrReplaceTrainerPayload, SearchTrainersPayload, Trainer, UpdateTrainerPayload } from "@/types/trainers";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post } from ".";
import type { Inventory } from "@/types/game";

export async function createTrainer(payload: CreateOrReplaceTrainerPayload): Promise<Trainer> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers" }).buildRelative();
  return (await post<CreateOrReplaceTrainerPayload, Trainer>(url, payload)).data;
}

export async function deleteTrainer(id: string): Promise<Trainer> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{id}" }).setParameter("id", id).buildRelative();
  return (await _delete<Trainer>(url)).data;
}

export async function getInventory(trainerId: string): Promise<Inventory> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/trainers/{trainerId}/inventory" }).setParameter("trainerId", trainerId).buildRelative();
  return (await get<Inventory>(url)).data;
}

export async function readTrainer(id: string): Promise<Trainer> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Trainer>(url)).data;
}

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

export async function updateTrainer(id: string, payload: UpdateTrainerPayload): Promise<Trainer> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateTrainerPayload, Trainer>(url, payload)).data;
}
