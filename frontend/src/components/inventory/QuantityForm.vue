<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import QuantityInput from "./QuantityInput.vue";
import { useForm } from "@/forms";
import type { InventoryItem } from "@/types/inventory";

const { t } = useI18n();

const props = defineProps<{
  item: InventoryItem;
  loading?: boolean | string;
}>();

const quantity = ref<number>(0);

useForm();

defineEmits<{
  (e: "add"): void;
  (e: "remove"): void;
  (e: "update", quantity: number): void;
}>();

watch(
  () => props.item,
  (item) => (quantity.value = item.quantity),
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="$emit('update', quantity)">
    <QuantityInput :min="0" v-model="quantity">
      <template #prepend>
        <TarButton :disabled="loading" icon="fas fa-minus" :loading="loading" :status="t('loading')" variant="danger" @click="$emit('remove')" />
      </template>
      <template #append>
        <TarButton :disabled="loading" icon="fas fa-floppy-disk" :loading="loading" :status="t('loading')" type="submit" />
        <TarButton :disabled="loading" icon="fas fa-plus" :loading="loading" :status="t('loading')" variant="success" @click="$emit('add')" />
      </template>
    </QuantityInput>
  </form>
</template>

<style scoped>
form {
  max-width: 240px;
}
</style>
