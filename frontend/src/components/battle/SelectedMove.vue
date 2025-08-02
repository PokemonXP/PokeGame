<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import ExternalIcon from "@/components/icons/ExternalIcon.vue";
import ExternalLink from "./ExternalLink.vue";
import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import type { PokemonMove } from "@/types/pokemon";
import { calculateStamina } from "@/helpers/pokemon";

const { n, t } = useI18n();

defineProps<{
  attack: PokemonMove;
}>();

type Mode = "description" | "notes";
const mode = ref<Mode>("description");
</script>

<template>
  <div>
    <div class="d-flex align-items-center justify-content-between">
      <h2 class="h6">{{ t("moves.use.selected") }}</h2>
      <div class="btn-group" role="group" aria-label="Move Display Mode">
        <TarButton :class="{ active: mode === 'description' }" icon="fas fa-book" :text="t('description')" @click="mode = 'description'" />
        <TarButton :class="{ active: mode === 'notes' }" icon="fas fa-note-sticky" :text="t('notes')" @click="mode = 'notes'" />
      </div>
    </div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("moves.select.label") }}</th>
          <th scope="col">
            {{ t("pokemon.type.label") }}
            <br />
            {{ t("moves.category.label") }}
          </th>
          <th scope="col">
            {{ t("moves.accuracy.label") }}
            <br />
            {{ t("moves.power") }}
          </th>
          <th scope="col">
            {{ t("moves.powerPoints.abbreviation") }}
            <br />
            {{ t("pokemon.stamina.label") }}
          </th>
          <th scope="col">
            <template v-if="mode === 'description'">{{ t("description") }}</template>
            <template v-else-if="mode === 'notes'">{{ t("notes") }}</template>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>
            <RouterLink :to="{ name: 'MoveEdit', params: { id: attack.move.id } }" target="_blank">
              <ExternalIcon /> {{ attack.move.displayName ?? attack.move.uniqueName }}
              <br v-if="attack.move.url || attack.move.displayName" />
              <ExternalLink v-if="attack.move.url" :href="attack.move.url" />
              <template v-else> <MoveIcon /> {{ attack.move.uniqueName }} </template>
            </RouterLink>
          </td>
          <td>
            <PokemonTypeImage :type="attack.move.type" height="20" />
            <br />
            <MoveCategoryBadge :category="attack.move.category" height="20" />
          </td>
          <td>
            <template v-if="attack.move.accuracy">{{ n(attack.move.accuracy / 100, "integer_percent") }}</template>
            <span v-else class="text-muted">{{ t("moves.accuracy.neverMisses") }}</span>
            <br />
            <template v-if="attack.move.power">{{ attack.move.power }}</template>
            <span v-else class="text-muted">{{ "—" }}</span>
          </td>
          <td>
            {{ attack.move.powerPoints }}
            <br />
            {{ calculateStamina(attack.move.powerPoints) }}
          </td>
          <td class="description">
            <template v-if="mode === 'description' && attack.move.description">{{ attack.move.description }}</template>
            <template v-else-if="mode === 'notes' && attack.move.notes">{{ attack.move.notes }}</template>
            <span v-else class="text-muted">{{ "—" }}</span>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
