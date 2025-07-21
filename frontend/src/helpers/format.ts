import type { Ability, Form, Species, Variety } from "@/types/pokemon";
import type { Item } from "@/types/items";
import type { Move } from "@/types/pokemon/moves";
import type { Trainer } from "@/types/trainers";

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

export function formatSpecies(species: Species): string {
  return species.displayName ? `${species.displayName} (${species.uniqueName})` : species.uniqueName;
}

export function formatTrainer(trainer: Trainer): string {
  return trainer.displayName ? `${trainer.displayName} (${trainer.uniqueName})` : trainer.uniqueName;
}

export function formatVariety(variety: Variety): string {
  return variety.displayName ? `${variety.displayName} (${variety.uniqueName})` : variety.uniqueName;
}

// TODO(fpion): unit tests
