<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { GrowthRate } from "@/types/pokemon";
import { getMaximumExperience } from "@/helpers/pokemon";

const { t } = useI18n();

withDefaults(
  defineProps<{
    growthRate: GrowthRate;
    id?: string;
  }>(),
  {
    id: "experience-table",
  },
);

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function close(): void {
  modalRef.value?.hide();
}
</script>

<template>
  <TarModal
    :close="t('actions.close')"
    :id="id"
    ref="modalRef"
    size="x-large"
    :title="`${t('pokemon.experience.table.title')} - ${t(`pokemon.growthRate.select.options.${growthRate}`)}`"
  >
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("pokemon.level.label") }}</th>
          <th scope="col">{{ t("pokemon.experience.label") }}</th>
          <th scope="col">{{ t("pokemon.level.label") }}</th>
          <th scope="col">{{ t("pokemon.experience.label") }}</th>
          <th scope="col">{{ t("pokemon.level.label") }}</th>
          <th scope="col">{{ t("pokemon.experience.label") }}</th>
          <th scope="col">{{ t("pokemon.level.label") }}</th>
          <th scope="col">{{ t("pokemon.experience.label") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="i in 25" :key="i">
          <td>{{ i }}</td>
          <td>{{ i === 1 ? 0 : getMaximumExperience(growthRate, i - 1) }}</td>
          <td>{{ i + 25 }}</td>
          <td>{{ getMaximumExperience(growthRate, i - 1 + 25) }}</td>
          <td>{{ i + 50 }}</td>
          <td>{{ getMaximumExperience(growthRate, i - 1 + 50) }}</td>
          <td>{{ i + 75 }}</td>
          <td>{{ getMaximumExperience(growthRate, i - 1 + 75) }}</td>
        </tr>
      </tbody>
    </table>
    <template #footer>
      <TarButton icon="fas fa-times" :text="t('actions.close')" variant="secondary" @click="close" />
    </template>
  </TarModal>
</template>
