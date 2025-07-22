<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonTypeImage from "./PokemonTypeImage.vue";
import type { MoveDisplayMove, PokemonMove } from "@/types/pokemon";

const { n, t } = useI18n();
const { parseBoolean, parseNumber } = parsingUtils;

const props = defineProps<{
  current?: boolean | string;
  loading?: boolean | string;
  mode: MoveDisplayMove;
  moves: PokemonMove[];
  selected?: number | string;
}>();

const isCurrent = computed<boolean>(() => parseBoolean(props.current) ?? false);
const selectedPosition = computed<number | undefined>(() => parseNumber(props.selected));

defineEmits<{
  (e: "remember", move: PokemonMove): void;
  (e: "selected", position: number): void;
  (e: "switch", destination: number): void;
}>();
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
          <th scope="col">{{ t("name.label") }}</th>
          <th scope="col">
            {{ t("pokemon.type.label") }}
            {{ "/" }}
            {{ t("moves.category.label") }}
          </th>
          <th scope="col">
            {{ t("moves.accuracy.label") }}
            {{ "/" }}
            {{ t("moves.power") }}
          </th>
          <th scope="col">{{ t("moves.powerPoints.label") }}</th>
          <th scope="col">{{ t("pokemon.move.learned.label") }}</th>
          <th scope="col">
            <template v-if="mode === 'actions'"></template>
            <template v-if="mode === 'description'">{{ t("description") }}</template>
            <template v-if="mode === 'notes'">{{ t("notes") }}</template>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in moves" :key="item.move.id" :class="{ 'table-primary': isCurrent && selectedPosition === index }">
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
            <RouterLink :to="{ name: 'MoveEdit', params: { id: item.move.id } }">
              <MoveIcon /> {{ item.move.displayName ?? item.move.uniqueName }}
              <template v-if="item.move.displayName">
                <br />
                {{ item.move.uniqueName }}
              </template>
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
            <br />
            <template v-if="item.move.power">{{ item.move.power }}</template>
            <span v-else class="text-muted">{{ "—" }}</span>
          </td>
          <td>
            {{ item.powerPoints.current }} / {{ item.powerPoints.maximum }}
            <br />
            {{ t("moves.powerPoints.format", { powerPoints: item.powerPoints.reference }) }}
          </td>
          <td>
            {{ t("pokemon.level.format", { level: item.level }) }}
            <br />
            {{ t(`pokemon.move.learningMethod.options.${item.method}`) }}
          </td>
          <td class="text">
            <template v-if="mode === 'actions'">
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
                :text="t('pokemon.move.remember')"
                variant="warning"
                @click="$emit('remember', item)"
              />
            </template>
            <template v-if="mode === 'notes'">
              <template v-if="item.move.notes">{{ item.move.notes }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </template>
            <template v-if="mode === 'description'">
              <template v-if="item.move.description">{{ item.move.description }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </template>
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("moves.empty") }}</p>
  </div>
</template>

<style scoped>
input[type="radio"] {
  transform: scale(1.4);
  margin-right: 0.75rem;
}

.text {
  max-width: 400px;
  word-wrap: break-word;
  white-space: normal;
}
</style>
