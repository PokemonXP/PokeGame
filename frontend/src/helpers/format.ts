import type { Ability, Form, Species, Variety } from "@/types/pokemon";

export function formatAbility(ability: Ability): string {
  return ability.displayName ? `${ability.displayName} (${ability.uniqueName})` : ability.uniqueName;
}

export function formatForm(form: Form): string {
  return form.displayName ? `${form.displayName} (${form.uniqueName})` : form.uniqueName;
}

export function formatSpecies(species: Species): string {
  return species.displayName ? `${species.displayName} (${species.uniqueName})` : species.uniqueName;
}

export function formatVariety(variety: Variety): string {
  return variety.displayName ? `${variety.displayName} (${variety.uniqueName})` : variety.uniqueName;
}
