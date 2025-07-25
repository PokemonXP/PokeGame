import type { Item } from "./items";

export type InventoryItem = {
  item: Item;
  quantity: number;
};

export type InventoryQuantityPayload = {
  quantity: number;
};
