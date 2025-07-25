import { urlUtils } from "logitar-js";

import type { InventoryItem, InventoryQuantityPayload } from "@/types/inventory";
import { _delete, get, post, put } from ".";

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

export async function removeItem(trainerId: string, itemId: string, payload: InventoryQuantityPayload): Promise<InventoryItem> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{trainerId}/inventory/{itemId}" })
    .setParameter("trainerId", trainerId)
    .setParameter("itemId", itemId)
    .setQuery("quantity", payload.quantity.toString())
    .buildRelative();
  return (await _delete<InventoryItem>(url)).data;
}

export async function updateItem(trainerId: string, itemId: string, payload: InventoryQuantityPayload): Promise<InventoryItem> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{trainerId}/inventory/{itemId}" })
    .setParameter("trainerId", trainerId)
    .setParameter("itemId", itemId)
    .buildRelative();
  return (await put<InventoryQuantityPayload, InventoryItem>(url, payload)).data;
}
