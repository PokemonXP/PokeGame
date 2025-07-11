import type { Locale } from "./i18n";

export type CurrentUser = {
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
  isAdmin: boolean;
};

export type GoogleSignInPayload = {
  token: string;
};

export type SignInPayload = {
  username: string;
  password: string;
};

export type UserProfile = {
  username: string;
  emailAddress?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  fullName?: string | null;
  birthdate?: string | null;
  gender?: string | null;
  locale?: Locale | null;
  timeZone?: string | null;
  picture?: string | null;
  createdOn: string;
  updatedOn: string;
  authenticatedOn?: string | null;
  passwordChangedOn?: string | null;
  isAdmin: boolean;
};
