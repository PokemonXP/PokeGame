import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Species } from "@/types/pokemon";
import { get } from ".";

export async function searchSpecies(): Promise<SearchResults<Species>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species" }).buildRelative(); // TODO(fpion): payload + query params
  return (await get<SearchResults<Species>>(url)).data;
}
