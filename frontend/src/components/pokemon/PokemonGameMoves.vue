<script setup lang="ts">
import { TarButton, TarCard } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import PokemonTypeImage from "./PokemonTypeImage.vue";
import type { MoveSummary, PokemonSummary } from "@/types/game";
import type { SwapPokemonMovesPayload } from "@/types/pokemon";
import { swapPokemonMoves } from "@/api/game/pokemon";

const { n, t } = useI18n();

const props = defineProps<{
  pokemon: PokemonSummary;
}>();

const isLoading = ref<boolean>(false);
const isSwapping = ref<boolean>(false);
const selected = ref<number>();

const selectedMove = computed<MoveSummary | undefined>(() => (typeof selected.value === "number" ? props.pokemon.moves[selected.value] : undefined));

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "swapped", indices: number[]): void;
}>();

async function swap(destination: number): Promise<void> {
  if (!isLoading.value && typeof selected.value === "number" && selected.value !== destination) {
    isLoading.value = true;
    try {
      const payload: SwapPokemonMovesPayload = {
        source: selected.value,
        destination,
      };
      await swapPokemonMoves(props.pokemon.id, payload);
      setTimeout(() => (selected.value = destination), 1);
      isSwapping.value = false;
      emit("swapped", [selected.value, destination]);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
function select(index: number): void {
  if (selected.value === index) {
    selected.value = undefined;
    isSwapping.value = false;
  } else if (isSwapping.value) {
    swap(index);
  } else {
    selected.value = index;
  }
}

watch(
  () => props.pokemon,
  () => (selected.value = undefined),
  { deep: true },
);
</script>

<template>
  <section>
    <TarCard
      v-for="(move, index) in pokemon.moves"
      :key="index"
      :class="{ clickable: true, 'mb-1': true, selected: selected === index }"
      @click="select(index)"
    >
      <span>
        <PokemonTypeImage class="me-1" height="32" :type="move.type" />
        <span class="ms-1">{{ move.name }}</span>
      </span>
      <span class="float-end">{{ move.currentPowerPoints }}/{{ move.maximumPowerPoints }}</span>
    </TarCard>
    <div v-if="selectedMove" class="mt-4">
      <h3 class="h5">{{ t("description") }}</h3>
      <table class="table table-striped">
        <tbody>
          <tr>
            <th scope="row">{{ t("moves.category.label") }}</th>
            <td>
              <MoveCategoryBadge :category="selectedMove.category" height="32" />
            </td>
          </tr>
          <tr>
            <th scope="row">{{ t("moves.accuracy.label") }}</th>
            <td>
              <template v-if="selectedMove.accuracy">{{ n(selectedMove.accuracy / 100, "integer_percent") }}</template>
              <span v-else class="text-muted">{{ t("moves.accuracy.neverMisses") }}</span>
            </td>
          </tr>
          <tr>
            <th scope="row">{{ t("moves.power") }}</th>
            <td>
              <template v-if="selectedMove.power">{{ n(selectedMove.power, "integer") }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
          </tr>
          <tr v-if="selectedMove.description">
            <td colspan="2">{{ selectedMove.description }}</td>
          </tr>
        </tbody>
      </table>
      <div class="mb-3">
        <TarButton
          class="me-1"
          :disabled="isLoading"
          icon="fas fa-rotate"
          :loading="isLoading"
          :outline="!isSwapping"
          :status="t('loading')"
          :text="t('pokemon.move.swap.label')"
          @click="isSwapping = !isSwapping"
        />
        <i v-if="isSwapping" class="ms-1"><CircleInfoIcon /> {{ t("pokemon.move.swap.help") }}</i>
      </div>
    </div>
  </section>
</template>

<style scoped>
th {
  width: 180px;
}
</style>
