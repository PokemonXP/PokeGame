<script setup lang="ts">
import { watch } from "vue";

import AbilityCard from "./AbilityCard.vue";
import type { AbilitySlot } from "@/types/abilities";
import type { FormAbilities } from "@/types/pokemon";
import { randomInteger } from "@/helpers/random";

const props = defineProps<{
  abilities: FormAbilities;
  modelValue: AbilitySlot;
}>();

const emit = defineEmits<{
  (e: "update:model-value", slot: AbilitySlot): void;
}>();

watch(
  () => props.abilities,
  (abilities) => {
    let slot: AbilitySlot = "Primary";
    if (abilities.secondary) {
      const value: number = randomInteger(1, 2);
      if (value === 2) {
        slot = "Secondary";
      }
    }
    emit("update:model-value", slot);
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div class="row mb-3">
    <div class="col">
      <AbilityCard
        class="d-flex flex-column h-100"
        :ability="abilities.primary"
        ability-slot="Primary"
        :selected="modelValue === 'Primary'"
        @selected="$emit('update:model-value', 'Primary')"
      />
    </div>
    <div v-if="abilities.secondary" class="col">
      <AbilityCard
        class="d-flex flex-column h-100"
        :ability="abilities.secondary"
        ability-slot="Secondary"
        :selected="modelValue === 'Secondary'"
        @selected="$emit('update:model-value', 'Secondary')"
      />
    </div>
    <div v-if="abilities.hidden" class="col">
      <AbilityCard
        class="d-flex flex-column h-100"
        :ability="abilities.hidden"
        ability-slot="Hidden"
        :selected="modelValue === 'Hidden'"
        @selected="$emit('update:model-value', 'Hidden')"
      />
    </div>
  </div>
</template>
