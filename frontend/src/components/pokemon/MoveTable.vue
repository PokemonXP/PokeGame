<script setup lang="ts">
import { TarBadge, TarButton } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonTypeImage from "./PokemonTypeImage.vue";
import type { PokemonMove } from "@/types/pokemon";
import { getMoveUrl } from "@/helpers/cms";

const { n, t } = useI18n();
const { parseBoolean, parseNumber } = parsingUtils;

const props = defineProps<{
  current?: boolean | string;
  loading?: boolean | string;
  moves: PokemonMove[];
  selected?: number | string;
}>();

const isCurrent = computed<boolean>(() => parseBoolean(props.current) ?? false);
const selectedPosition = computed<number | undefined>(() => parseNumber(props.selected));

defineEmits<{
  (e: "selected", position: number): void;
  (e: "relearn", move: PokemonMove): void;
  (e: "switch", destination: number): void;
}>();

// TODO(fpion): Mastered
</script>

<template>
  <div>
    <table v-if="moves.length" class="table table-striped">
      <thead>
        <tr>
          <template v-if="isCurrent">
            <th scope="col"></th>
            <th scope="col">#</th>
          </template>
          <th scope="col">{{ t("pokemon.move.name") }}</th>
          <th scope="col">{{ t("pokemon.move.typeAndCategory") }}</th>
          <th scope="col">{{ t("pokemon.move.accuracy") }} / {{ t("pokemon.move.power") }}</th>
          <th scope="col">{{ t("pokemon.move.powerPoints.label") }}</th>
          <th scope="col">{{ t("pokemon.move.learned.label") }}</th>
          <th scope="col">{{ t("pokemon.move.description") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(move, index) in moves" :key="move.move.id" :class="{ 'table-primary': isCurrent && selectedPosition === index }">
          <template v-if="isCurrent">
            <td>
              <div class="form-check">
                <input
                  :checked="selectedPosition === index"
                  class="form-check-input"
                  name="selected-move"
                  :id="`select-move-${index}`"
                  type="radio"
                  @change="$emit('selected', index)"
                />
              </div>
            </td>
            <td>{{ index + 1 }}</td>
          </template>
          <td>
            <a :href="getMoveUrl(move.move)" target="_blank">
              <MoveIcon /> {{ move.move.displayName ?? move.move.uniqueName }}
              <template v-if="move.move.displayName">
                <br />
                {{ move.move.uniqueName }}
              </template>
            </a>
          </td>
          <td>
            <PokemonTypeImage :type="move.move.type" />
            <br />
            <MoveCategoryBadge :category="move.move.category" />
          </td>
          <td>
            <template v-if="move.move.accuracy">{{ n(move.move.accuracy / 100, "integer_percent") }}</template>
            <span v-else class="text-muted">{{ "—" }}</span>
            <br />
            <template v-if="move.move.power">{{ move.move.power }}</template>
            <span v-else class="text-muted">{{ "—" }}</span>
          </td>
          <td>
            {{ move.powerPoints.current }} / {{ move.powerPoints.maximum }}
            <br />
            {{ t("pokemon.move.powerPoints.format", { powerPoints: move.powerPoints.reference }) }}
          </td>
          <td>
            {{ t("pokemon.level.format", { level: move.level }) }}
            <template v-if="move.technicalMachine">
              <br />
              <TarBadge>{{ t("pokemon.move.learned.technicalMachine") }}</TarBadge>
            </template>
          </td>
          <td class="description">
            <template v-if="move.move.description">{{ move.move.description }}</template>
            <span v-else class="text-muted">{{ "—" }}</span>
          </td>
          <td>
            <TarButton
              v-if="isCurrent"
              :disabled="typeof selectedPosition !== 'number' || selectedPosition === index || loading"
              icon="fas fa-rotate"
              :loading="loading"
              :status="t('loading')"
              :text="t('pokemon.move.switch')"
              @click="$emit('switch', index)"
            />
            <TarButton
              v-else
              :disabled="typeof selectedPosition !== 'number' || loading"
              icon="fas fa-rotate"
              :loading="loading"
              :status="t('loading')"
              :text="t('pokemon.move.relearn')"
              variant="warning"
              @click="$emit('relearn', move)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("pokemon.move.empty") }}</p>
  </div>
</template>

<style scoped>
.description {
  max-width: 400px;
  word-wrap: break-word;
  white-space: normal;
}

input[type="radio"] {
  transform: scale(1.4);
  margin-right: 0.75rem;
}
</style>
