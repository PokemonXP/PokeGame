import { urlUtils } from "logitar-js";

import type { PokemonSheet, PokemonSummary } from "@/types/pokemon/game";
import { get } from "..";

export async function getPokemonList(trainerId: string, box?: number): Promise<PokemonSheet[]> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/trainers/{trainerId}/pokemon" })
    .setParameter("trainerId", trainerId)
    .setQuery("box", box?.toString() ?? "")
    .buildRelative();
  return (await get<PokemonSheet[]>(url)).data;
} // TODO(fpion): rename to getPokemon

export async function getSummary(id: string): Promise<PokemonSummary> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/pokemon/{id}/summary" }).setParameter("id", id).buildRelative();
  return (await get<PokemonSummary>(url)).data;
}
