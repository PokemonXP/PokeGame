import { urlUtils } from "logitar-js";

import type { InventoryItem, InventoryQuantityPayload } from "@/types/inventory";
import { get, post } from ".";

export async function addItem(trainerId: string, itemId: string, payload: InventoryQuantityPayload): Promise<InventoryItem> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{trainerId}/inventory/{itemId}" })
    .setParameter("trainerId", trainerId)
    .setParameter("itemId", itemId)
    .buildRelative();
  return (await post<InventoryQuantityPayload, InventoryItem>(url, payload)).data;
}

export async function readInventory(trainerId: string): Promise<InventoryItem[]> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{trainerId}/inventory" }).setParameter("trainerId", trainerId).buildRelative();
  return (await get<InventoryItem[]>(url)).data;
}
