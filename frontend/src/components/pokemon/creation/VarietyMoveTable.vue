<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import type { VarietyMove } from "@/types/pokemon";
import { getMoveUrl } from "@/helpers/cms";

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
        <th scope="col">{{ t("pokemon.move.name") }}</th>
        <th scope="col">{{ t("pokemon.move.typeAndCategory") }}</th>
        <th scope="col">{{ t("pokemon.move.accuracy") }}</th>
        <th scope="col">{{ t("pokemon.move.power") }}</th>
        <th scope="col">{{ t("pokemon.move.powerPoints.label") }}</th>
        <th scope="col">{{ t("pokemon.move.description") }}</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="item in sortedMoves" :key="item.move.id">
        <td>{{ item.level || t("pokemon.move.learningMethod.options.Evolving") }}</td>
        <td>
          <a :href="getMoveUrl(item.move)" target="_blank">
            <template v-if="item.move.displayName">
              {{ item.move.displayName }}
              <br />
            </template>
            {{ item.move.uniqueName }}
          </a>
        </td>
        <td>
          <PokemonTypeImage :type="item.move.type" />
          <br />
          <MoveCategoryBadge :category="item.move.category" />
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

<style scoped>
.description {
  max-width: 400px;
  word-wrap: break-word;
  white-space: normal;
}
</style>
