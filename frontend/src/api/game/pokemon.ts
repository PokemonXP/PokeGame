import { urlUtils } from "logitar-js";

import type { NicknamePokemonPayload, PokemonCard, PokemonSummary } from "@/types/game";
import { get, patch } from "..";

export async function getPokemon(trainerId: string, box?: number): Promise<PokemonCard[]> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/trainers/{trainerId}/pokemon" })
    .setParameter("trainerId", trainerId)
    .setQuery("box", box?.toString() ?? "")
    .buildRelative();
  return (await get<PokemonCard[]>(url)).data;
}

export async function getSummary(id: string): Promise<PokemonSummary> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/pokemon/{id}/summary" }).setParameter("id", id).buildRelative();
  return (await get<PokemonSummary>(url)).data;
}

export async function nicknamePokemon(id: string, payload: NicknamePokemonPayload): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/pokemon/{id}/nickname" }).setParameter("id", id).buildRelative();
  await patch<NicknamePokemonPayload, void>(url, payload);
}
