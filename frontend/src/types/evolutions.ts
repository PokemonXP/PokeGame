import type { Aggregate, Change } from "./aggregate";
import type { Item } from "./items";
import type { Move } from "./moves";
import type { PokemonGender } from "./pokemon";
import type { Form } from "./pokemon-forms";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceEvolutionPayload = {
  source: string;
  target: string;
  trigger: EvolutionTrigger;
  item?: string;
  level: number;
  friendship: boolean;
  gender?: PokemonGender;
  heldItem?: string;
  knownMove?: string;
  location?: string;
  timeOfDay?: TimeOfDay;
};

export type Evolution = Aggregate & {
  source: Form;
  target: Form;
  trigger: EvolutionTrigger;
  item?: Item | null;
  level: number;
  friendship: boolean;
  gender?: PokemonGender | null;
  heldItem?: Item | null;
  knownMove?: Move;
  location?: string | null;
  timeOfDay?: TimeOfDay | null;
};

export type EvolutionSort = "CreatedOn" | "Level" | "UpdatedOn";

export type EvolutionSortOption = SortOption & {
  field: EvolutionSort;
};

export type EvolutionTrigger = "Level" | "Item" | "Trade";

export type SearchEvolutionsPayload = SearchPayload & {
  sourceId?: string;
  targetId?: string;
  trigger?: EvolutionTrigger;
  sort: EvolutionSortOption[];
};

export type TimeOfDay = "Day" | "Evening" | "Morning" | "Night";

export type UpdateEvolutionPayload = {
  level?: number;
  friendship?: boolean;
  gender?: Change<PokemonGender>;
  heldItem?: Change<string>;
  knownMove?: Change<string>;
  location?: Change<string>;
  timeOfDay?: Change<TimeOfDay>;
};
