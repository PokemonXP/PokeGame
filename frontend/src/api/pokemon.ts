import { urlUtils } from "logitar-js";

import type { CreatePokemonPayload, Pokemon } from "@/types/pokemon";
import { get, post } from ".";

export async function createPokemon(payload: CreatePokemonPayload): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon" }).buildRelative();
  return (await post<CreatePokemonPayload, Pokemon>(url, payload)).data;
}

export async function readPokemon(id: string): Promise<Pokemon> {
  const url: string = new urlUtils.UrlBuilder({ path: "/pokemon/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Pokemon>(url)).data;
}
