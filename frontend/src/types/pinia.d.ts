import "pinia";

declare module "pinia" {
  export interface DefineStoreOptionsBase<> {
    persist?: boolean | PersistedStateOptions;
  }
}
