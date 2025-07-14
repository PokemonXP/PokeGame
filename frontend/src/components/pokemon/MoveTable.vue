<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { Move } from "@/types/pokemon/moves";
import { getMoveUrl } from "@/helpers/cms";

const { t } = useI18n();

defineProps<{
  moves: Move[];
}>();

defineEmits<{
  (e: "down", index: number): void;
  (e: "removed", index: number): void;
  (e: "up", index: number): void;
}>();
</script>

<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th scope="col">{{ t("pokemon.move.name") }}</th>
        <th scope="col">{{ t("pokemon.move.typeAndCategory") }}</th>
        <th scope="col">{{ t("pokemon.move.accuracy") }}</th>
        <th scope="col">{{ t("pokemon.move.power") }}</th>
        <th scope="col">{{ t("pokemon.move.powerPoints") }}</th>
        <th scope="col">{{ t("pokemon.move.description") }}</th>
        <th scope="col"></th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="(move, index) in moves" :key="move.id">
        <td>
          <a :href="getMoveUrl(move)" target="_blank">
            <template v-if="move.displayName">
              {{ move.displayName }}
              <br />
            </template>
            {{ move.uniqueName }}
          </a>
        </td>
        <td>
          {{ t(`pokemon.type.select.options.${move.type}`) }}
          <br />
          {{ t(`pokemon.move.category.options.${move.category}`) }}
        </td>
        <td>
          <template v-if="move.accuracy">{{ move.accuracy }}</template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td>
          <template v-if="move.power">{{ move.power }}</template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td>{{ move.powerPoints }}</td>
        <td class="description">
          <template v-if="move.description">{{ move.description }}</template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td>
          <TarButton class="me-1" :disabled="index === 0" icon="fas fa-arrow-up" @click="$emit('up', index)" />
          <TarButton class="mx-1" :disabled="index === moves.length - 1" icon="fas fa-arrow-down" @click="$emit('down', index)" />
          <TarButton class="ms-1" icon="fas fa-times" variant="danger" @click="$emit('removed', index)" />
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
