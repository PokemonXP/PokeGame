<script setup lang="ts">
import { TarProgress } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const { n, t } = useI18n();

const props = defineProps<{
  gender?: number | null;
}>();

const isGenderUnknown = computed<boolean>(() => typeof props.gender !== "number");
const percentage = computed<number>(() => (typeof props.gender !== "number" || props.gender < 0 ? 0 : Math.min(props.gender, 8) / 8));
</script>

<template>
  <div>
    <template v-if="isGenderUnknown">{{ t("pokemon.gender.unknown") }}</template>
    <template v-else>
      <div class="d-flex align-items-center gap-2">
        <font-awesome-icon icon="fas fa-venus" />
        <div class="flex-grow-1">
          <TarProgress :aria-label="t('pokemon.gender.select.label')" :label="n(percentage, 'percent')" min="0" max="100" :value="percentage * 100" />
        </div>
        <font-awesome-icon icon="fas fa-mars" />
      </div>
    </template>
  </div>
</template>
