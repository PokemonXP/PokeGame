import type { Aggregate, Change } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceRegionPayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  url?: string;
  notes?: string;
};

export type Region = Aggregate & {
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  url?: string | null;
  notes?: string | null;
};

export type RegionSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type RegionSortOption = SortOption & {
  field: RegionSort;
};

export type SearchRegionsPayload = SearchPayload & {
  sort: RegionSortOption[];
};

export type UpdateRegionPayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  url?: Change<string>;
  notes?: Change<string>;
};
