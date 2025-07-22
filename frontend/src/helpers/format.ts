import type { Ability } from "@/types/abilities";
import type { Form, Species, Variety } from "@/types/pokemon";
import type { Item } from "@/types/items";
import type { Move } from "@/types/moves";
import type { Region } from "@/types/regions";
import type { Trainer } from "@/types/trainers";
import type { UserSummary } from "@/types/users";

export function formatAbility(ability: Ability): string {
  return ability.displayName ? `${ability.displayName} (${ability.uniqueName})` : ability.uniqueName;
}

export function formatForm(form: Form): string {
  return form.displayName ? `${form.displayName} (${form.uniqueName})` : form.uniqueName;
}

export function formatItem(item: Item): string {
  return item.displayName ? `${item.displayName} (${item.uniqueName})` : item.uniqueName;
}

export function formatMove(move: Move): string {
  return move.displayName ? `${move.displayName} (${move.uniqueName})` : move.uniqueName;
}

export function formatRegion(region: Region): string {
  return region.displayName ? `${region.displayName} (${region.uniqueName})` : region.uniqueName;
}

export function formatSpecies(species: Species): string {
  return species.displayName ? `${species.displayName} (${species.uniqueName})` : species.uniqueName;
}

export function formatTrainer(trainer: Trainer): string {
  return trainer.displayName ? `${trainer.displayName} (${trainer.uniqueName})` : trainer.uniqueName;
}

export function formatUser(user: UserSummary): string {
  return user.fullName ? `${user.fullName} (${user.uniqueName})` : user.uniqueName;
}

export function formatVariety(variety: Variety): string {
  return variety.displayName ? `${variety.displayName} (${variety.uniqueName})` : variety.uniqueName;
}

// TODO(fpion): unit tests
