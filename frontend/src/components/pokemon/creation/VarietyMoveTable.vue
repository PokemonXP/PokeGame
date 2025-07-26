<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import type { VarietyMove } from "@/types/varieties";

const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  level: number;
  moves: VarietyMove[];
}>();

const sortedMoves = computed<VarietyMove[]>(() =>
  orderBy(
    props.moves
      .filter((item) => !item.level || item.level <= props.level)
      .map((item) => ({ ...item, sort: [item.level.toString().padStart(3, "0"), item.move.displayName ?? item.move.uniqueName].join("_") })),
    "sort",
  ),
);
</script>

<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th scope="col">{{ t("pokemon.level.label") }}</th>
        <th scope="col">{{ t("name.label") }}</th>
        <th scope="col">
          {{ t("pokemon.type.label") }}
          {{ "/" }}
          {{ t("moves.category.label") }}
        </th>
        <th scope="col">{{ t("moves.accuracy.label") }}</th>
        <th scope="col">{{ t("moves.power") }}</th>
        <th scope="col">{{ t("moves.powerPoints.label") }}</th>
        <th scope="col">{{ t("description") }}</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="item in sortedMoves" :key="item.move.id">
        <td>{{ item.level || t("pokemon.move.learningMethod.options.Evolving") }}</td>
        <td>
          <RouterLink :to="{ name: 'MoveEdit', params: { id: item.move.id } }">
            <template v-if="item.move.displayName">
              {{ item.move.displayName }}
              <br />
            </template>
            {{ item.move.uniqueName }}
          </RouterLink>
        </td>
        <td>
          <PokemonTypeImage height="20" :type="item.move.type" />
          <br />
          <MoveCategoryBadge :category="item.move.category" height="20" />
        </td>
        <td>
          <template v-if="item.move.accuracy">{{ n(item.move.accuracy / 100, "integer_percent") }}</template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td>
          <template v-if="item.move.power">{{ item.move.power }}</template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td>{{ item.move.powerPoints }}</td>
        <td class="description">
          <template v-if="item.move.description">{{ item.move.description }}</template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
      </tr>
    </tbody>
  </table>
</template>
