import type { Aggregate, Change } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceTrainerPayload = {
  license: string;
  uniqueName: string;
  displayName?: string;
  description?: string;
  gender: TrainerGender;
  money: number;
  userId?: string;
  sprite?: string;
  url?: string;
  notes?: string;
};

export type SearchTrainersPayload = SearchPayload & {
  gender?: TrainerGender;
  userId?: string;
  sort: TrainerSortOption[];
};

export type Trainer = Aggregate & {
  license: string;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  gender: TrainerGender;
  money: number;
  partySize: number;
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

export type UpdateTrainerPayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  gender?: TrainerGender;
  money?: number;
  userId?: Change<string>;
  sprite?: Change<string>;
  url?: Change<string>;
  notes?: Change<string>;
};
