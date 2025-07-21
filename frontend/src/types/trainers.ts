import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

export type SearchTrainersPayload = SearchPayload & {
  gender?: TrainerGender;
  userId?: string;
  sort: TrainerSortOption[];
};

export type Trainer = Aggregate & {
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  gender: TrainerGender;
  license: string;
  money: number;
  userId?: string | null;
  sprite?: string | null;
  url?: string | null;
  notes?: string | null;
};

export type TrainerGender = "Female" | "Male";

export type TrainerSort = "CreatedOn" | "DisplayName" | "License" | "Money" | "UniqueName" | "UpdatedOn";

export type TrainerSortOption = SortOption & {
  field: TrainerSort;
};
