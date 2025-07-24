import { urlUtils } from "logitar-js";

import type { PokemonSheet } from "@/types/pokemon/game";
import { get } from "..";

export async function getPokemonList(trainerId: string, box?: number): Promise<PokemonSheet[]> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/trainers/{trainerId}/pokemon" })
    .setParameter("trainerId", trainerId)
    .setQuery("box", box?.toString() ?? "")
    .buildRelative();
  return (await get<PokemonSheet[]>(url)).data;
}
