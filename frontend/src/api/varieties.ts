import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Variety } from "@/types/pokemon";
import { get } from ".";

export async function searchVarieties(speciesId: string): Promise<SearchResults<Variety>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/{speciesId}/varieties" }).setParameter("speciesId", speciesId).buildRelative(); // TODO(fpion): payload + query params
  return (await get<SearchResults<Variety>>(url)).data;
}
