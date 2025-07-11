import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Form } from "@/types/pokemon";
import { get } from ".";

export async function searchForms(varietyId: string): Promise<SearchResults<Form>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{varietyId}/forms" }).setParameter("varietyId", varietyId).buildRelative(); // TODO(fpion): payload + query params
  return (await get<SearchResults<Form>>(url)).data;
}
