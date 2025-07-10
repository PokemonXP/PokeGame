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
