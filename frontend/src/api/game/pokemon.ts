import { urlUtils } from "logitar-js";

import type { ChangePokemonItemPayload, NicknamePokemonPayload, PokemonCard, PokemonSummary } from "@/types/game";
import type { SwapPokemonMovesPayload } from "@/types/pokemon";
import { get, patch } from "..";

export async function changeHeldItem(id: string, payload: ChangePokemonItemPayload): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/pokemon/{id}/item/held" }).setParameter("id", id).buildRelative();
  await patch<ChangePokemonItemPayload, void>(url, payload);
}

export async function depositPokemon(id: string): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/pokemon/{id}/deposit" }).setParameter("id", id).buildRelative();
  await patch<void, void>(url);
}

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

export async function swapPokemon(sourceId: string, destinationId: string): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/pokemon/{sourceId}/swap/{destinationId}" })
    .setParameter("sourceId", sourceId)
    .setParameter("destinationId", destinationId)
    .buildRelative();
  await patch<void, void>(url);
}

export async function swapPokemonMoves(id: string, payload: SwapPokemonMovesPayload): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/pokemon/{id}/moves/swap" }).setParameter("id", id).buildRelative();
  await patch<SwapPokemonMovesPayload, void>(url, payload);
}

export async function withdrawPokemon(id: string): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/game/pokemon/{id}/withdraw" }).setParameter("id", id).buildRelative();
  await patch<void, void>(url);
}
