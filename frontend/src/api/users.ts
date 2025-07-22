import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { UserSummary } from "@/types/users";
import { get } from ".";

export async function searchUsers(): Promise<SearchResults<UserSummary>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/users" }).buildRelative();
  return (await get<SearchResults<UserSummary>>(url)).data;
}
