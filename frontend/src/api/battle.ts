import { urlUtils } from "logitar-js";

import type { CreateBattlePayload, Battle } from "@/types/battle";
import { post } from ".";

export async function createBattle(payload: CreateBattlePayload): Promise<Battle> {
  const url: string = new urlUtils.UrlBuilder({ path: "/battles" }).buildRelative();
  return (await post<CreateBattlePayload, Battle>(url, payload)).data;
}
