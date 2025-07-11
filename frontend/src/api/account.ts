import { urlUtils } from "logitar-js";

import type { CurrentUser, GoogleSignInPayload, SignInPayload, UserProfile } from "@/types/account";
import { get, post } from ".";

export async function getProfile(): Promise<UserProfile> {
  const url: string = new urlUtils.UrlBuilder({ path: "/profile" }).buildRelative();
  return (await get<UserProfile>(url)).data;
}

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
