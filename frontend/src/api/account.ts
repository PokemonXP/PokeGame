import { urlUtils } from "logitar-js";

import { patch } from ".";

export async function signOut(): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sign/out" }).buildRelative();
  await patch(url);
}
