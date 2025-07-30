import { urlUtils } from "logitar-js";

import type { Inventory } from "@/types/game";
import type { TrainerSheet } from "@/types/game";
import { get } from "..";

export async function getInventory(trainerId: string): Promise<Inventory> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/trainers/{trainerId}/inventory" }).setParameter("trainerId", trainerId).buildRelative();
  return (await get<Inventory>(url)).data;
}

export async function getTrainerSheet(id: string): Promise<TrainerSheet> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/trainers/{id}" }).setParameter("id", id).buildRelative();
  return (await get<TrainerSheet>(url)).data;
}

export async function getTrainerSheets(): Promise<TrainerSheet[]> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/trainers" }).buildRelative();
  return (await get<TrainerSheet[]>(url)).data;
}
