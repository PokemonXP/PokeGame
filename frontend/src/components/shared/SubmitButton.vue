<script setup lang="ts">
import { TarButton, type ButtonType, type ButtonVariant } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    icon?: string;
    loading?: boolean | string;
    status?: string;
    text?: string;
    type?: ButtonType;
    variant?: ButtonVariant;
  }>(),
  {
    icon: "fas fa-floppy-disk",
    status: "loading",
    text: "actions.save",
    type: "submit",
  },
);
</script>

<template>
  <TarButton :disabled="loading" :icon="icon" :loading="loading" :status="t(status)" :text="t(text)" :type="type" :variant="variant" />
</template>
