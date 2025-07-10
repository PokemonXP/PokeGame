import { urlUtils } from "logitar-js";

import type { CurrentUser, GoogleSignInPayload, SignInPayload } from "@/types/account";
import { post } from ".";

export async function signIn(payload: SignInPayload): Promise<CurrentUser> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sign/in" }).buildRelative();
  return (await post<SignInPayload, CurrentUser>(url, payload)).data;
}

export async function signInGoogle(payload: GoogleSignInPayload): Promise<CurrentUser> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sign/in/google" }).buildRelative();
  return (await post<GoogleSignInPayload, CurrentUser>(url, payload)).data;
}

export async function signOut(): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sign/out" }).buildRelative();
  await post(url);
}
