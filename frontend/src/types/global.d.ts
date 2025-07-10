// src/global.d.ts
export {};

declare global {
  interface Window {
    google: typeof google;
  }

  namespace google {
    namespace accounts.id {
      interface CredentialResponse {
        credential: string;
        select_by: string;
        clientId: string;
      }

      function initialize(config: { client_id: string; callback: (response: CredentialResponse) => void; locale: string }): void;

      function renderButton(
        parent: HTMLElement,
        options: {
          theme?: "outline" | "filled_blue" | "filled_black";
          size?: "small" | "medium" | "large";
          text?: "signin_with" | "signup_with" | "continue_with" | "sign_in_with";
          shape?: "rectangular" | "pill" | "circle" | "square";
          logo_alignment?: "left" | "center";
          width?: number;
        },
      ): void;

      function prompt(): void;
    }
  }
}
